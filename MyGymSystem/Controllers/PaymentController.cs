using iText.Kernel.Colors;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using MyGymSystem.Context;
using MyGymSystem.Models;
using iText.Kernel.Pdf;
using iText.Layout.Element;
using iText.Layout.Properties;
using MailKit.Security;
using MimeKit;
using MailKit.Net.Smtp;
using iText.Kernel.Colors;
using iText.Layout;
using NuGet.DependencyResolver;

namespace MyGymSystem.Controllers
{
	public class PaymentController : Controller
	{
		private readonly GymDbContext _context;
		private readonly IWebHostEnvironment _hostEnvironment;
		private readonly IConfiguration _configuration;

		public PaymentController(
			GymDbContext context,
			IWebHostEnvironment hostEnvironment,
			IConfiguration configuration)
		{
			_context = context;
			_hostEnvironment = hostEnvironment;
			_configuration = configuration;
		}




        [HttpGet]
        public IActionResult Pay(long planId, long? trainerId)
        {
            var plan = _context.Membershipplans.FirstOrDefault(p => p.Planid == planId);
            if (plan == null)
                return NotFound("Plan not found");
			if(trainerId!= null)
			{
				var trainer = _context.Trainers.FirstOrDefault(x => x.Trainerid == trainerId);
                if (trainer != null)
                    ViewBag.TrainerName = trainer.Firstname + " " + trainer.Lastname;
            }

            ViewBag.PlanName = plan.Planname;
            ViewBag.PlanPrice = plan.Price;
            ViewBag.PlanID = plan.Planid;
            ViewBag.TrainerID = trainerId; // optional
            ViewBag.PaymentDate = DateTime.Now;

            return View();
        }



        [HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Pay(long? planId,long? trainerId, [Bind("Paymentid,Paymentmethod,Paymentstatus,Externaltransactionid")] Payment payment)
		{
            ModelState.Remove(nameof(Payment.Amountpaid));
            ModelState.Remove(nameof(Payment.Paymentdate));

            if (planId == null)
				return NotFound();

			

			if (!ModelState.IsValid)
			{
				foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
				{
					Console.WriteLine(error.ErrorMessage);
				}
				return View(payment);
			}



            var memberId = HttpContext.Session.GetInt32("MNId");
            if (memberId == null)
                return RedirectToAction("Login", "Auth");

            var plan = await _context.Membershipplans
                    .FirstOrDefaultAsync(p => p.Planid == planId);

            if (plan == null)
                return NotFound("Plan not found");



            var isPersonal = string.Equals(plan.Plantype, "Personal", StringComparison.OrdinalIgnoreCase);
            Trainer? trainer = null;

			if (isPersonal)
			{
				if (trainerId is null) return BadRequest("A trainer is required for Personal plans.");
				trainer = await _context.Trainers.FirstOrDefaultAsync(t => t.Trainerid == trainerId.Value);
				if (trainer is null) return NotFound("Trainer not found.");
			}
			else
			{
				trainerId = null; // ignore any trainer sent for non-personal plans
			}

            // 3) Optional: prevent overlapping active subs for the same plan
            var hasActive = await _context.Subscriptions.AnyAsync(s =>
                s.Memberid == memberId.Value &&
                s.Planid == plan.Planid &&
                (s.Status ?? false) &&
                s.Enddate >= DateTime.UtcNow);
            if (hasActive)
                return BadRequest("You already have an active subscription for this plan.");

            // Get member details for invoice
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Memberid == memberId.Value);

            if (member == null)
                throw new Exception("Member not found");


            // Begin transaction

            await using var tx = await _context.Database.BeginTransactionAsync();

            try
            {
				

				// Process payment
				payment.Paymentdate = DateTime.Now;
				payment.Amountpaid = plan.Price;
				payment.Paymentstatus = "Completed"; // Set initial status

				_context.Payments.Add(payment);
				await _context.SaveChangesAsync();

				// Create subscription
				var subscription = new Subscription
				{
					Paymentid = payment.Paymentid,
					Memberid = memberId.Value,
					Planid = planId,
					Startdate = DateTime.Now,
					Enddate = DateTime.Now.AddMonths(plan.Durationmonths),
					Status = true,
					Createdat = DateTime.Now
                };

				_context.Subscriptions.Add(subscription);

                // Personal: deactivate previous active trainer and assign new
                if (isPersonal && trainer is not null)
                {
                    // deactivate any active assignment for this member
                    var actives = await _context.MemberTrainer
                        .Where(x => x.MemberId == memberId.Value && x.IsActive)
                        .ToListAsync();

                    foreach (var a in actives) a.IsActive = false;

                    // assign new trainer
                    _context.MemberTrainer.Add(new MemberTrainer
                    {
                        MemberId = memberId.Value,
                        TrainerId = trainer.Trainerid,
                        AssignedAt = DateTime.UtcNow,
                        IsActive = true
                    });
                }

                // Generate and save invoice
                var invoicePath = await GenerateInvoiceAsync(member, payment, plan,trainer);

				var invoice = new Invoice
				{
					Paymentid = payment.Paymentid,
					Memberid = memberId.Value,
					Filepath = invoicePath,
					Invoicedate = DateTime.Now
				};

					await	_context.Invoices.AddAsync(invoice);
				await _context.SaveChangesAsync();
				await tx.CommitAsync();

                try
                {
                    // Send email if member has email address

                    if (!string.IsNullOrWhiteSpace(member.Email))
                        await SendInvoiceEmailAsync(member, invoicePath);
                }
                catch { /* log if needed; do not throw */ }
             

				TempData["SuccessMessage"] = "Payment processed successfully!";
				return RedirectToAction("Index", "Home");
			}
			catch (Exception ex)
			{
                await tx.RollbackAsync();
                ModelState.AddModelError("", "An error occurred: " + ex.Message);
                ViewData["Planid"] = new SelectList(_context.Membershipplans, "Planid", "Planname", planId);

             
				return View(payment);
			}
		}

		private async Task<string> GenerateInvoiceAsync(Member member, Payment payment, Membershipplan plan,Trainer ? trainer)
		{
			var wwwRootPath = _hostEnvironment.WebRootPath;
			var invoiceFolder = Path.Combine(wwwRootPath, "Invoices");
			Directory.CreateDirectory(invoiceFolder);
			var invoicePath = Path.Combine(invoiceFolder, $"Invoice_{payment.Paymentid}.pdf");

			using (var writer = new PdfWriter(invoicePath))
			using (var pdf = new PdfDocument(writer))
			using (var document = new Document(pdf))
			{
				document.SetMargins(40, 40, 40, 40);

				// Add gym logo/header
				document.Add(new Paragraph("FITNESS GYM")
					.SetTextAlignment(TextAlignment.CENTER)
					.SetFontSize(24)
					.SetBold());

				document.Add(new Paragraph("INVOICE")
					.SetTextAlignment(TextAlignment.CENTER)
					.SetFontSize(20)
					.SetMarginBottom(30));

				// Invoice details table
				var table = new Table(2)
					.SetWidth(UnitValue.CreatePercentValue(100))
					.SetMarginBottom(20);

                AddTableRow(table, "Invoice Number:", payment.Paymentid.ToString());
                AddTableRow(table, "Date:", payment.Paymentdate.ToString("dd/MM/yyyy"));
                AddTableRow(table, "Member Name:", $"{member.Firstname} {member.Lastname}");
                AddTableRow(table, "Member ID:", member.Memberid.ToString());
                AddTableRow(table, "Plan Name:", plan.Planname);
                AddTableRow(table, "Amount Paid:", $"{payment.Amountpaid:F2} JOD");
                AddTableRow(table, "Payment Method:", payment.Paymentmethod);
                
                if (trainer != null && !string.IsNullOrWhiteSpace(trainer.Firstname))
                {
                    AddTableRow(table, "Personal Trainer:", $"{trainer.Firstname} {trainer.Lastname}");
                }


                document.Add(table);

				// Terms and conditions
				document.Add(new Paragraph("Terms and Conditions:")
					.SetBold()
					.SetMarginTop(20));

				document.Add(new Paragraph(
					"1. This invoice confirms your membership subscription.\n" +
					"2. Membership is non-transferable and non-refundable.\n" +
					"3. Please retain this invoice for your records.")
					.SetFontSize(10));

				// Add footer
				document.Add(new Paragraph("Thank you for choosing Fitness Gym!")
					.SetTextAlignment(TextAlignment.CENTER)
					.SetMarginTop(30)
					.SetItalic());
			}

			return  $"/Invoices/Invoice_{payment.Paymentid}.pdf";
		}

		private static void AddTableRow(Table table, string label, string value)
		{
			table.AddCell(new Cell()
				.Add(new Paragraph(label))
				.SetBold()
				.SetBackgroundColor(ColorConstants.LIGHT_GRAY)
				.SetPadding(5));

			table.AddCell(new Cell()
				.Add(new Paragraph(value))
				.SetPadding(5));
		}
        private async Task SendInvoiceEmailAsync(Member member, string invoicePath)
        {
            var fullInvoicePath = Path.Combine(_hostEnvironment.WebRootPath, invoicePath.TrimStart('/'));
            var message = new MimeMessage();

            var emailSettings = _configuration.GetSection("EmailSettings");

            message.From.Add(new MailboxAddress("Fitness Gym", emailSettings["SenderEmail"]));
            message.To.Add(new MailboxAddress($"{member.Firstname} {member.Lastname}", member.Email));
            message.Subject = "Your Fitness Gym Invoice";

            var builder = new BodyBuilder
            {
                HtmlBody = $@"
                <h2>Thank you for your payment, {member.Firstname}!</h2>
                <p>Please find your invoice attached to this email.</p>
                <p>Membership Details:</p>
                <ul>
                    <li>Member ID: {member.Memberid}</li>
                    <li>Invoice Date: {DateTime.Now:dd/MM/yyyy}</li>
                </ul>
                <p>If you have any questions, please don't hesitate to contact us.</p>
                <br>
                <p>Best regards,<br>Fitness Gym Team</p>",
                TextBody = "Thank you for your payment. Your invoice is attached to this email."
            };

            builder.Attachments.Add(fullInvoicePath);
            message.Body = builder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(
                emailSettings["SmtpServer"],
                int.Parse(emailSettings["SmtpPort"]),
                SecureSocketOptions.StartTls);
            await client.AuthenticateAsync(
                emailSettings["SmtpUsername"],
                emailSettings["SmtpPassword"]);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
        //private async Task SendInvoiceEmailAsync(Member member, string invoicePath)
        //{
        //    var fullInvoicePath = Path.Combine(_hostEnvironment.WebRootPath, invoicePath.TrimStart('/'));
        //    var message = new MimeMessage();
        //    var emailSettings = _configuration.GetSection("EmailSettings");

        //    var senderEmail = emailSettings["SenderEmail"];

        //    // Validate sender email (from appsettings.json)
        //    if (!MailboxAddress.TryParse(senderEmail, out var sender))
        //        throw new Exception($"Invalid sender email in appsettings.json: {senderEmail}");

        //    // Validate recipient email (from database)
        //    if (string.IsNullOrWhiteSpace(member.Email) || !MailboxAddress.TryParse(member.Email, out var recipient))
        //    {
        //        Console.WriteLine($"Invalid or missing member email: '{member.Email}' — skipping email send.");
        //        return; // Skip sending if invalid
        //    }

        //    message.From.Add(new MailboxAddress("Fitness Gym", sender.Address));
        //    message.To.Add(new MailboxAddress($"{member.Firstname} {member.Lastname}", recipient.Address));
        //    message.Subject = "Your Fitness Gym Invoice";

        //    var builder = new BodyBuilder
        //    {
        //        HtmlBody = $@"
        //    <h2>Thank you for your payment, {member.Firstname}!</h2>
        //    <p>Please find your invoice attached to this email.</p>
        //    <p>Membership Details:</p>
        //    <ul>
        //        <li>Member ID: {member.Memberid}</li>
        //        <li>Invoice Date: {DateTime.Now:dd/MM/yyyy}</li>
        //    </ul>
        //    <p>Best regards,<br>Fitness Gym Team</p>",
        //        TextBody = "Thank you for your payment. Your invoice is attached."
        //    };

        //    builder.Attachments.Add(fullInvoicePath);
        //    message.Body = builder.ToMessageBody();

        //    using var client = new MailKit.Net.Smtp.SmtpClient();
        //    await client.ConnectAsync(
        //        emailSettings["SmtpServer"],
        //        int.Parse(emailSettings["SmtpPort"]),
        //        SecureSocketOptions.StartTls);

        //    await client.AuthenticateAsync(
        //        emailSettings["SmtpUsername"],
        //        emailSettings["SmtpPassword"]);

        //    await client.SendAsync(message);
        //    await client.DisconnectAsync(true);
        //}

        [HttpGet]
		public async Task<IActionResult> DownloadInvoice(int invoiceId)
		{
			var invoice = await _context.Invoices
				.FirstOrDefaultAsync(i => i.Invoiceid == invoiceId);

			if (invoice == null)
				return NotFound();

			var filepath = Path.Combine(_hostEnvironment.WebRootPath, invoice.Filepath.TrimStart('/'));

			if (!System.IO.File.Exists(filepath))
				return NotFound("Invoice file not found");

			return PhysicalFile(filepath, "application/pdf", Path.GetFileName(filepath));
		}












	}
}
