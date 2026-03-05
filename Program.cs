using Microsoft.Extensions.DependencyInjection;
using RideSharing.Application.Interfaces;
using RideSharing.Application.Services;
using RideSharing.Infrastructure.Persistence;
using RideSharing.Infrastructure.UnitOfWork;
using RideSharing.Presentation.ConsoleUI;

// ---------------------------------------------------------------
// Composition Root — wire up the dependency injection container.
// All services and repositories are registered as singletons so
// that the shared in-memory lists inside JsonUnitOfWork remain
// consistent throughout the application lifetime.
// ---------------------------------------------------------------

var services = new ServiceCollection();

// Infrastructure
services.AddSingleton<JsonFileService>();
services.AddSingleton<IUnitOfWork, JsonUnitOfWork>();

// Application Services
services.AddSingleton<PaymentService>();
services.AddSingleton<AuthService>();
services.AddSingleton<RideService>();
services.AddSingleton<RatingService>();
services.AddSingleton<ReportService>();

// Presentation
services.AddSingleton<PassengerMenu>();
services.AddSingleton<DriverMenu>();
services.AddSingleton<AdminMenu>();
services.AddSingleton<MainMenu>();

var provider = services.BuildServiceProvider();

// Resolve the top-level menu and start the application.
provider.GetRequiredService<MainMenu>().Start();
