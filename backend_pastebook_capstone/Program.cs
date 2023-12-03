using backend_pastebook_capstone.AuthenticationService.Authenticator;
using backend_pastebook_capstone.AuthenticationService.Models;
using backend_pastebook_capstone.AuthenticationService.Repository;
using backend_pastebook_capstone.AuthenticationService.TokenGenerators;
using backend_pastebook_capstone.Data;
using backend_pastebook_capstone.Repository;
using backend_pastebook_capstone.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace backend_pastebook_capstone
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var builder = WebApplication.CreateBuilder(args);


			builder.Services.AddControllers();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowAllOrigins",
					builder =>
					{
						builder.AllowAnyOrigin()
							   .AllowAnyMethod()
							   .AllowAnyHeader();
					});
			});

			builder.Services.Configure<FormOptions>(options =>
			{
				options.ValueLengthLimit = int.MaxValue;
				options.MultipartBodyLengthLimit = 512 * 1024 * 1024;
				options.MultipartHeadersLengthLimit = int.MaxValue;
			});
			builder.WebHost.ConfigureKestrel(options =>
			{
				options.Limits.MaxRequestBodySize = 512 * 1024 * 1024;
			});

			builder.Services.Configure<IISServerOptions>(options =>
			{
				options.AllowSynchronousIO = true;
			});

			AuthenticationConfiguration authenticationConfiguration = new AuthenticationConfiguration();
			builder.Configuration.Bind("Authentication", authenticationConfiguration);
			builder.Services.AddSingleton(authenticationConfiguration);

			builder.Services.AddScoped<BcryptPasswordHasher>();
			builder.Services.AddScoped<UserRepository>();
			builder.Services.AddScoped<PostRepository>();
			builder.Services.AddScoped<TimelineRepository>();
			builder.Services.AddScoped<CommentRepository>();
			builder.Services.AddScoped<FriendRepository>();
			builder.Services.AddScoped<NotificationRepository>();
			builder.Services.AddScoped<AlbumRepository>();
			builder.Services.AddScoped<PhotoRepository>();

			builder.Services.AddScoped<AccessTokenRepository>();
			builder.Services.AddScoped<TokenGenerator>();
			builder.Services.AddScoped<AccessTokenGenerator>();
			builder.Services.AddScoped<TokenGenerator>();
			builder.Services.AddScoped<Authenticator>();

			builder.Services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Debug));
			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o =>
			{
				o.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
				{
					IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationConfiguration.AccessTokenSecret)),
					ValidIssuer = authenticationConfiguration.Issuer,
					ValidAudience = authenticationConfiguration.Audience,
					ValidateIssuerSigningKey = true,
					ValidateIssuer = true,
					ValidateAudience = true,
					ClockSkew = TimeSpan.Zero
				};
			});

			string connectionString = "Server=localhost;port=3306;Database=Pastebook_Database;User=root;";
			builder.Services.AddDbContext<CapstoneDBContext>(options => options.UseMySQL(connectionString));

			builder.Services.AddLogging(logging =>
			{
				logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Debug);
			});


			var app = builder.Build();

			if (app.Environment.IsDevelopment())
			{
				app.UseSwagger();
				app.UseSwaggerUI();
			}

			app.UseHttpsRedirection();
			
			app.UseCors("AllowAllOrigins");

			/*app.UseStaticFiles(new StaticFileOptions
			{
				FileProvider = new PhysicalFileProvider(
				Path.Combine(Directory.GetCurrentDirectory(), "PastebookData", "photos")
				),
				RequestPath = "/photos"
			});*/


			app.UseAuthorization();

			app.MapControllers();

			app.Run();
		}
	}
}