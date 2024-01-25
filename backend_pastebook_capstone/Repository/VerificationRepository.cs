using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Models;
using backend_pastebook_capstone.Services;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;

namespace backend_pastebook_capstone.Repository
{
    public class VerificationRepository
    {
        public readonly CapstoneDBContext _context;

        public VerificationRepository(CapstoneDBContext context)
        {
            _context = context;
        }

        public Verification? GetVerificationByEmail(string email)
        {
            return _context.Verification.FirstOrDefault(x => x.Email == email);
        }

        public bool SendVerificationEmail(string email)
        {
            var senderEmail = "teametivacpastebook@gmail.com";
            //teametivac123
            var senderPassword = "gcew fzxh mloo gmji";

            var word = RandomGenerator.GenerateRandomWord();

            Verification? verification = GetVerificationByEmail(email);
            if (verification == null)
            {
                verification = new Verification
                {
                    Email = email,
                    VerificationCode = word
                };
            }
            else
            {
                verification.VerificationCode = word;
            }

            UpdateVerification(verification);

            var message = new MailMessage(senderEmail, email)
            {
                Subject = $"Verify your email, {email}!",
                IsBodyHtml = true,
                Body =
                $@"
					<html>
					<head>
					<title>Pastebook Email Confirmation</title>
					<style>
					body {{
					  font-family: sans-serif;
					  margin: 0;
					  padding: 0;
					}}

					.container {{
					  width: 600px;
					  margin: 0 auto;
                      flex: 1;
                        justify-content: center
					}}

					h1, h2, h3 {{
					  text-align: center;
					  font-size: 30px;
					  margin-top: 40px;
					}}

					p {{
					  font-size: 16px;
					  line-height: 1.5;
					}}

					a {{
					  color: #fff;
					  background-color: #f9a113;
					  padding: 10px 20px;
					  border-radius: 4px;
					  text-decoration: none;
					}}

					.footer {{
					  text-align: center;
					  font-size: 12px;
					  margin-top: 40px;
					}}
					</style>
					</head>
					<body>
					  <div class=""container"">
						<img style='width: 100;' src = 'https://cdn.discordapp.com/attachments/1174240282951303239/1176866765209337886/Logo1_dark.PNG?ex=65706d95&is=655df895&hm=62cdfca8d61e6fd565803260716f0493e37f4d764e82c5b5d8e11f9e861783e3&' alt = 'Pastebook Logo'>
						<h1>Email Confirmation</h1>
						
						<h3>
						  Simply input the code below to verify.
						</h3>
						<h1>
						  {word}
						</h1>
						<div class=""footer"">
						  Copyright © 2023 Team Etivac. All rights reserved. Email sent by Pastebook.com.
						</div>
					  </div>
					</body>
					</html>
				"
            };


            var smtpClient = new SmtpClient("smtp.gmail.com")
            {
                Port = 587,
                Credentials = new NetworkCredential(senderEmail, senderPassword),
                EnableSsl = true,
            };

            try
            {
                smtpClient.SendMailAsync(message);
                return true;
            }
            catch (Exception)
            {
                return false;
            }


        }
        public bool CheckIfCodeIsValid(string email, string code)
        {
            return _context.Verification.Any(v => v.Email == email && v.VerificationCode == code);
        }

        public void AddVerification(Verification verification)
        {
            _context.Verification.Add(verification);
            _context.SaveChanges();
        }
        public void UpdateVerification(Verification verification)
        {
            _context.Verification.Update(verification);
            _context.SaveChanges();
        }
        public void RemoveVerification(Verification verification)
        {
            _context.Verification.Remove(verification);
            _context.SaveChanges();
        }
    }
}
