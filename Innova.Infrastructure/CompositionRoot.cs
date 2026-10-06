using System.Reflection;
using DbUp;
using DbUp.Engine;
using FluentValidation;
using Innova.Application.Abstractions.Events;
using Innova.Application.Abstractions.Messaging;
using Innova.Application.Abstractions.Persistence;
using Innova.Application.Abstractions.Services;
using Innova.Application.Behaviours;
using Innova.Application.Billing.Folio.Commands.OpenFolio;
using Innova.Application.Billing.Folio.Commands.PostAdjustment;
using Innova.Application.Billing.Folio.Commands.PostCharge;
using Innova.Application.Billing.Folio.Commands.RecordPayment;
using Innova.Application.Billing.Folio.Commands.SettleFolio;
using Innova.Application.Billing.Folio.Commands.VoidFolio;
using Innova.Application.Dispatchers;
using Innova.Application.FrontDesk.Stay.Commands.AddOccupant;
using Innova.Application.FrontDesk.Stay.Commands.CheckIn;
using Innova.Application.FrontDesk.Stay.Commands.CheckOut;
using Innova.Application.GuestManagement.Guest.Commands.ChangeGuestIdentityDocument;
using Innova.Application.GuestManagement.Guest.Commands.DeactivateGuest;
using Innova.Application.GuestManagement.Guest.Commands.ReactivateGuest;
using Innova.Application.GuestManagement.Guest.Commands.RegisterGuest;
using Innova.Application.GuestManagement.Guest.Commands.UpdateGuestContactDetails;
using Innova.Application.HouseKeeping.HouseKeeping.Commands.AssignTask;
using Innova.Application.HouseKeeping.HouseKeeping.Commands.CompleteTask;
using Innova.Application.HouseKeeping.HouseKeeping.Commands.ScheduleTask;
using Innova.Application.Identity.Commands.AuthenticateUser;
using Innova.Application.Identity.Commands.ChangePassword;
using Innova.Application.Identity.Commands.DeactivateUser;
using Innova.Application.Identity.Commands.ReactivateUser;
using Innova.Application.Identity.Commands.RegisterUser;
using Innova.Application.Reservations.GroupBooking.Commands.AttachReservationToGroup;
using Innova.Application.Reservations.GroupBooking.Commands.DetachReservationFromGroup;
using Innova.Application.Reservations.GroupBooking.Commands.OpenGroupBooking;
using Innova.Application.Reservations.Reservation.Commands.BookReservation;
using Innova.Application.Reservations.Reservation.Commands.CancelReservation;
using Innova.Application.Reservations.Reservation.Commands.ConfirmReservation;
using Innova.Application.Reservations.Reservation.Commands.MarkNoShow;
using Innova.Application.RoomInventory.Room.Commands.CreateRoom;
using Innova.Application.RoomInventory.Room.Commands.ReturnRoomToService;
using Innova.Application.RoomInventory.Room.Commands.TakeRoomOutOfService;
using Innova.Application.RoomInventory.RoomTypeAllotment.Commands.CreateRoomTypeAllotment;
using Innova.Application.RoomInventory.RoomTypeDefinition.Commands.DefineRoomType;
using Innova.Application.Services;
using Innova.Application.Settings;
using Innova.Application.Shared.Abstractions;
using Innova.Domain.Billing.Repositories;
using Innova.Domain.FrontDesk.Repositories;
using Innova.Domain.GuestManagement.Repositories;
using Innova.Domain.HouseKeeping.Repositories;
using Innova.Domain.Identity.Repositories;
using Innova.Domain.Reservations.Repositories;
using Innova.Domain.RoomInventory.Repositories;
using Innova.Infrastructure.Billing.Folio.Repository;
using Innova.Infrastructure.FrontDesk.Stay.Repository;
using Innova.Infrastructure.GuestManagement.Guest.Queries;
using Innova.Infrastructure.GuestManagement.Guest.Repository;
using Innova.Infrastructure.HouseKeeping.HouseKeeping.Repository;
using Innova.Infrastructure.Identity.Security;
using Innova.Infrastructure.Identity.User.Repository;
using Innova.Infrastructure.Persistence;
using Innova.Infrastructure.Persistence.Connections;
using Innova.Infrastructure.Persistence.Transactions;
using Innova.Infrastructure.Reservations.GroupBooking.Repository;
using Innova.Infrastructure.Reservations.Reservation.Repository;
using Innova.Infrastructure.RoomInventory.Room.Repository;
using Innova.Infrastructure.RoomInventory.RoomType.Repository;
using Innova.Infrastructure.RoomInventory.RoomTypeAllotment.Repository;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MvvmCross.IoC;
using Serilog;

namespace Innova.Infrastructure
{
    // ══════════════════════════════════════════════════════════════════════
    // CompositionRoot — the single place in the entire application where
    // every concrete type is constructed and mapped to its abstraction.
    //
    // Nothing outside this class should ever call `new` on an infrastructure
    // type. ViewModels, command handlers, and application services only ever
    // see interfaces — they never know what implements them.
    //
    // Registration order matters:
    //   1. Configuration   — everything else reads from it
    //   2. Logging         — everything else can log
    //   3. Database        — migrations run before any repository is used
    //   4. Persistence     — UnitOfWork and connection factory
    //   5. Repositories    — depend on persistence
    //   6. App services    — depend on repositories and domain services
    //   7. Validators      — depend on nothing, scanned by convention
    //   8. Command Handlers — depend on app services and validators
    //  9. Query Handlers — depend on app services
    //  10. Dispatchers     — depend on the IoC container to resolve handlers
    //  11. Core services   - depend on nothing
    //  12. ViewModels      — depend on dispatchers
    // ══════════════════════════════════════════════════════════════════════
    public static class CompositionRoot
    {
        public static void RegisterAllDependencies( IMvxIoCProvider iocProvider,
                                                    IConfigurationRoot configuration,
                                                    Assembly wpfAssembly,
                                                    Action<IMvxIoCProvider> registerPresentationServices )
        {
            RegisterConfiguration(iocProvider, configuration);
            RegisterLogging(iocProvider);
            RunDatabaseMigrations(iocProvider);
            RegisterPersistence(iocProvider);
            RegisterRepositories(iocProvider);
            RegisterApplicationServices(iocProvider);
            RegisterValidators(iocProvider);
            RegisterCommandHandlers(iocProvider);
            RegisterQueryHandlers(iocProvider);
            RegisterDispatchers(iocProvider);
            RegisterCoreServices(iocProvider, registerPresentationServices);
            RegisterViewModels(iocProvider, wpfAssembly);
        }

        // ══════════════════════════════════════════════════════════════════
        // SECTION 1 — Configuration
        //
        // Registers the already-built IConfigurationRoot as a singleton.
        // Built once in App.xaml.cs and passed in — never rebuilt here.
        // Every other section that needs a config value resolves this.
        // ══════════════════════════════════════════════════════════════════

#region Configuration

        private static void RegisterConfiguration( IMvxIoCProvider iocProvider, IConfigurationRoot configuration )
        {
            HotelSettings hotelSettings = configuration
                                          .GetSection(HotelSettings.SectionName)
                                          .Get<HotelSettings>()
                                          ?? throw new InvalidOperationException("Hotel settings are missing from appsettings.json.");

            if (string.IsNullOrWhiteSpace(hotelSettings.DefaultCurrency))
                throw new InvalidOperationException(
                    "Hotel:DefaultCurrency is required.");

            string currency = hotelSettings
                              .DefaultCurrency.Trim()
                              .ToUpperInvariant();

            if (currency.Length != 3)
                throw new InvalidOperationException(
                    "Hotel:DefaultCurrency must be a valid 3-letter currency code.");

            hotelSettings = new HotelSettings
                            {
                                DefaultCurrency = currency
                            };

            iocProvider.RegisterSingleton(hotelSettings);
        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 2 — Logging
        //
        // Configures Serilog as the logging backend and bridges it into
        // Microsoft.Extensions.Logging so ILogger<T> injection works in
        // behaviours and command handlers without any ASP.NET Core dependency.
        //
        // Log.Logger is set as the static global logger — this means anywhere
        // in the codebase that calls Log.Information(...) will use Serilog,
        // even in code that doesn't use dependency injection.
        // ══════════════════════════════════════════════════════════════════

#region Logging

        private static void RegisterLogging( IMvxIoCProvider iocProvider )
        {

            ILoggerFactory loggerFactory = LoggerFactory.Create(builder =>
                                                                    builder.AddSerilog(Log.Logger, true));

            iocProvider.RegisterSingleton(loggerFactory);

            // Open generic registration — MVVMCross resolves
            // ILogger<GuestRepository> as Logger<GuestRepository> automatically
            iocProvider.RegisterType(typeof(ILogger<>), typeof(Logger<>));
        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 3 — Database Migrations
        //
        // DbUp scans the Infrastructure assembly for embedded .sql migration
        // scripts and runs any that haven't been applied yet, in filename
        // order. DbUp tracks applied scripts in a journal table it creates
        // automatically (SchemaVersions).
        //
        // On a brand-new machine (e.g. a recruiter running the project for
        // the first time), all  migrations run automatically. On subsequent
        // runs, only new migrations run. The developer never needs to manually
        // run SQL scripts.
        // ══════════════════════════════════════════════════════════════════

#region Migrations

        private static void RunDatabaseMigrations( IMvxIoCProvider iocProvider )
        {
            IConfigurationRoot configuration =
                iocProvider.Resolve<IConfigurationRoot>();

            string connectionString = configuration.GetConnectionString("InnovaDB")
                                      ?? throw new InvalidOperationException(
                                          "InnovaDB connection string is missing from appsettings.json.");

            EnsureDatabase.For.SqlDatabase(connectionString);

            DatabaseUpgradeResult result = DeployChanges
                                           .To
                                           .SqlDatabase(connectionString)
                                           .WithScriptsEmbeddedInAssembly(
                                               typeof(CompositionRoot).Assembly,
                                               script => script.Contains(".Persistence.Migrations."))
                                           .LogToConsole()
                                           .Build()
                                           .PerformUpgrade();

            if (!result.Successful)
            {
                throw new InvalidOperationException(
                    $"Database migration failed: {result.Error.Message}",
                    result.Error);
            }
        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 4 — Persistence
        //
        // ISqlConnectionFactory knows how to create a SqlConnection.
        // IUnitOfWork and IDbConnectionProvider are both implemented by
        // UnitOfWork — registered as a factory (not singleton) so a fresh
        // instance is created per command, giving each command its own
        // connection and transaction.
        //
        // The key insight: IUnitOfWork and IDbConnectionProvider resolve
        // to the SAME UnitOfWork instance within one resolution chain —
        // TransactionCommandHandler opens the transaction via IUnitOfWork,
        // and repositories read the same connection via IDbConnectionProvider.
        //
        // This works in MVVMCross because RegisterType with a factory
        // function is called once per resolution, and since both interfaces
        // are resolved in the same object graph for a single command
        // execution, MVVMCross resolves UnitOfWork once and reuses it.
        // ══════════════════════════════════════════════════════════════════

#region Persistence

        private static void RegisterPersistence( IMvxIoCProvider iocProvider )
        {
            IConfigurationRoot configuration = iocProvider.Resolve<IConfigurationRoot>();

            string connectionString = configuration
                                          .GetConnectionString("InnovaDB")
                                      ?? throw new InvalidOperationException(
                                          "InnovaDB connection string is missing.");

            // Factory creates a new SqlConnection on demand
            ISqlConnectionFactory connectionFactory = new SqlConnectionFactory(connectionString);
            iocProvider.RegisterSingleton(connectionFactory);

            // IUnitOfWork / IDbConnectionProvider are no longer resolved via an
            // independent factory each — see BuildPipeline below, which
            // constructs exactly ONE UnitOfWork per command pipeline and
            // publishes it through AmbientUnitOfWork so every dependency
            // resolved while that pipeline is being built (e.g. a repository's
            // IDbConnectionProvider) captures a reference to the SAME instance
            // TransactionCommandHandler will Begin/Commit/Rollback.
            iocProvider.RegisterType<IUnitOfWork>(() =>
                                                      AmbientUnitOfWork.Current
                                                      ?? throw new InvalidOperationException(
                                                          "IUnitOfWork resolved outside of a command pipeline build. " +
                                                          "It is only valid while CompositionRoot.BuildPipeline is constructing a command's handler graph."));

            iocProvider.RegisterType<IDbConnectionProvider>(() =>
                                                                AmbientUnitOfWork.Current
                                                                ?? throw new InvalidOperationException(
                                                                    "IDbConnectionProvider resolved outside of a command pipeline build. " +
                                                                    "Repositories must only be constructed as part of a command's handler graph."));
        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 5 — Repositories
        //
        // Each domain repository interface maps to its infrastructure
        // implementation. Registered as transient (RegisterType) since each
        // repository borrows the connection from the ambient IDbConnectionProvider
        // and has no state of its own beyond that.
        //
        // Repositories are registered explicitly rather than by convention
        // because the domain interface and the infrastructure implementation
        // live in different assemblies — convention scanning by name suffix
        // would require both to be visible in the same assembly scan.
        // ══════════════════════════════════════════════════════════════════

#region Repositories

        private static void RegisterRepositories( IMvxIoCProvider ioCProvider )
        {
            ioCProvider.RegisterType<IGuestRepository, GuestRepository>();
            ioCProvider.RegisterType<IRoomRepository, RoomRepository>();
            ioCProvider.RegisterType<IRoomTypeDefinitionRepository, RoomTypeDefinitionRepository>();
            ioCProvider.RegisterType<IRoomTypeAllotmentRepository, RoomTypeAllotmentRepository>();
            ioCProvider.RegisterType<IGroupBookingRepository, GroupBookingRepository>();
            ioCProvider.RegisterType<IReservationRepository, ReservationRepository>();
            ioCProvider.RegisterType<IStayRepository, StayRepository>();
            ioCProvider.RegisterType<IHouseKeepingTaskRepository, HouseKeepingTaskRepository>();
            ioCProvider.RegisterType<IFolioRepository, FolioRepository>();
            ioCProvider.RegisterType<IUserRepository, UserRepository>();
        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 6 — Application Services
        //
        // It orchestrates use cases by coordinating
        // repositories, domain services, and aggregate factories.
        // They are registered against their interface so command handlers
        // can depend on abstractions.
        //
        // Registered as transient — they hold no state and borrow
        // repositories which are themselves transient.
        // ══════════════════════════════════════════════════════════════════

#region Application Services

        private static void RegisterApplicationServices( IMvxIoCProvider ioCProvider )
        {
            ioCProvider.RegisterType<IGuestManagementService, GuestManagementService>();
            ioCProvider.RegisterType<IRoomService, RoomService>();
            ioCProvider.RegisterType<IRoomTypeService, RoomTypeService>();
            ioCProvider.RegisterType<IRoomAvailabilityService, RoomAvailabilityService>();
            ioCProvider.RegisterType<IGroupBookingService, GroupBookingService>();
            ioCProvider.RegisterType<IReservationService, ReservationService>();
            ioCProvider.RegisterType<IStayService, StayService>();
            ioCProvider.RegisterType<IHouseKeepingService, HouseKeepingService>();
            ioCProvider.RegisterType<IBillingAssemblyService, BillingAssemblyService>();

            ioCProvider.RegisterType<ICurrentUserContext, CurrentUserContext>();
            ioCProvider.RegisterType<IPasswordHasher, PasswordHasher>();
            ioCProvider.RegisterType<IAuditLogger, AuditLogger>();
        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 7 — Validators
        //
        // FluentValidation validators are scanned by convention from the
        // Application assembly. Any class ending in "Validator" that
        // implements IValidator<T> is registered against that interface.
        //
        // The ValidatingCommandHandler resolves IEnumerable<IValidator<T>>
        // for a given command — MVVMCross returns an empty enumerable if
        // no validators are registered, which ValidatingCommandHandler
        // handles gracefully by skipping validation.
        // ══════════════════════════════════════════════════════════════════

#region Validators

        private static void RegisterValidators( IMvxIoCProvider iocProvider )
        {
            Assembly applicationAssembly =
                typeof(RegisterGuestCommand).Assembly;

            applicationAssembly
                .CreatableTypes()
                .EndingWith("Validator")
                .AsInterfaces()
                .RegisterAsLazySingleton();
        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 8 — Command Handlers
        //
        // This is the heart of the composition root. Every command gets
        // a fully assembled pipeline:
        //
        //   LoggingCommandHandler        ← outermost: logs start/end/errors
        //      AuthorizationCommandHandler ← checks user permissions
        //          ValidatingCommandHandler ← runs FluentValidation before the handler
        //              TransactionCommandHandler  ← opens DB transaction, commits or rolls back
        //                  ActualCommandHandler   ← innermost: does the real work
        //
        // The pipeline is built by the private helper BuildPipeline<T,R>.
        // ViewModels and dispatchers only ever see ICommandHandler<T,R> —
        // they are completely unaware the pipeline exists.
        //
        // Note: commands that return Guid (Register, Book, Open, Issue, Draft)
        // use ICommand<Guid>. Commands that return nothing use ICommand<Unit>.

#region Command Handlers

        private static void RegisterCommandHandlers( IMvxIoCProvider ioCProvider )
        {
            // ── Guest Management ──────────────────────────────────────
            BuildPipeline(
                ioCProvider,
                () => new RegisterGuestCommandHandler(
                    ioCProvider.Resolve<IGuestManagementService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new UpdateGuestContactDetailsCommandHandler(
                    ioCProvider.Resolve<IGuestManagementService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new ChangeGuestIdentityDocumentCommandHandler(
                    ioCProvider.Resolve<IGuestManagementService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>(),
                    ioCProvider.Resolve<IGuestRepository>()));

            BuildPipeline(
                ioCProvider,
                () => new DeactivateGuestCommandHandler(
                    ioCProvider.Resolve<IGuestManagementService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>(),
                    ioCProvider.Resolve<IGuestRepository>()));

            BuildPipeline(
                ioCProvider,
                () => new ReactivateGuestCommandHandler(
                    ioCProvider.Resolve<IGuestManagementService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>(),
                    ioCProvider.Resolve<IGuestRepository>()));

            // ── Room Inventory ──────────────────────────────────────
            // Room commands
            BuildPipeline(
                ioCProvider,
                () => new CreateRoomCommandHandler(
                    ioCProvider.Resolve<IRoomService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new ReturnRoomToServiceCommandHandler(
                    ioCProvider.Resolve<IRoomService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new TakeRoomOutOfServiceCommandHandler(
                    ioCProvider.Resolve<IRoomService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            //Room Type commands
            BuildPipeline(
                ioCProvider,
                () => new DefineRoomTypeCommandHandler(
                    ioCProvider.Resolve<IRoomTypeService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));


            //Room Type commands
            BuildPipeline(
                ioCProvider,
                () => new CreateRoomTypeAllotmentCommandHandler(
                    ioCProvider.Resolve<IRoomAvailabilityService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            // ── Reservations ──────────────────────────────────────
            // Group Bookings commands
            BuildPipeline(
                ioCProvider,
                () => new OpenGroupBookingCommandHandler(
                    ioCProvider.Resolve<IGroupBookingService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new AttachReservationToGroupCommandHandler(
                    ioCProvider.Resolve<IGroupBookingService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new DetachReservationFromGroupCommandHandler(
                    ioCProvider.Resolve<IGroupBookingService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            // Reservation commands
            BuildPipeline(
                ioCProvider,
                () => new BookReservationCommandHandler(
                    ioCProvider.Resolve<IReservationService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new ConfirmReservationCommandHandler(
                    ioCProvider.Resolve<IReservationService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new CancelReservationCommandHandler(
                    ioCProvider.Resolve<IReservationService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new MarkNoShowCommandHandler(
                    ioCProvider.Resolve<IReservationService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            // ── Front Desk ──────────────────────────────────────
            BuildPipeline(
                ioCProvider,
                () => new CheckInCommandHandler(
                    ioCProvider.Resolve<IStayService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new CheckOutCommandHandler(
                    ioCProvider.Resolve<IStayService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new AddOccupantCommandHandler(
                    ioCProvider.Resolve<IStayService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            // ── House Keeping ──────────────────────────────────────
            BuildPipeline(
                ioCProvider,
                () => new ScheduleTaskCommandHandler(
                    ioCProvider.Resolve<IHouseKeepingService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new AssignTaskCommandHandler(
                    ioCProvider.Resolve<IHouseKeepingService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new CompleteTaskCommandHandler(
                    ioCProvider.Resolve<IHouseKeepingService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            // ── Billing ──────────────────────────────────────
            BuildPipeline(
                ioCProvider,
                () => new OpenFolioCommandHandler(
                    ioCProvider.Resolve<IBillingAssemblyService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new RecordPaymentCommandHandler(
                    ioCProvider.Resolve<IBillingAssemblyService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new SettleFolioCommandHandler(
                    ioCProvider.Resolve<IBillingAssemblyService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new VoidFolioCommandHandler(
                    ioCProvider.Resolve<IBillingAssemblyService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new PostChargeCommandHandler(
                    ioCProvider.Resolve<IBillingAssemblyService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            BuildPipeline(
                ioCProvider,
                () => new PostAdjustmentCommandHandler(
                    ioCProvider.Resolve<IBillingAssemblyService>(),
                    ioCProvider.Resolve<IDomainEventDispatcher>()));

            // ── Authorization ───────────────────────────────────────────────────
            BuildPipeline(
                ioCProvider,
                () => new AuthenticateUserHandler(
                    ioCProvider.Resolve<IUserRepository>(),
                    ioCProvider.Resolve<IPasswordHasher>(),
                    ioCProvider.Resolve<ICurrentUserContext>()));

            BuildPipeline(
                ioCProvider,
                () => new RegisterUserHandler(
                    ioCProvider.Resolve<IUserRepository>(),
                    ioCProvider.Resolve<IPasswordHasher>()));

            BuildPipeline(
                ioCProvider,
                () => new DeactivateUserHandler(
                    ioCProvider.Resolve<IUserRepository>(),
                    ioCProvider.Resolve<ICurrentUserContext>()));

            BuildPipeline(
                ioCProvider,
                () => new ReactivateUserHandler(
                    ioCProvider.Resolve<IUserRepository>(),
                    ioCProvider.Resolve<ICurrentUserContext>()));

            BuildPipeline(
                ioCProvider,
                () => new ChangePasswordHandler(
                    ioCProvider.Resolve<IUserRepository>(),
                    ioCProvider.Resolve<IPasswordHasher>(),
                    ioCProvider.Resolve<ICurrentUserContext>()));
        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 9 — Query Handlers
        //
        // This is in charge of registering query handlers by convention.
        // Every query handler implements IQueryHandler<TQuery, TResult>

#region Query Handlers

        private static void RegisterQueryHandlers( IMvxIoCProvider iocProvider )
        {
            Assembly infrastructureAssembly = typeof(GetAllGuestQueryHandler).Assembly;

            infrastructureAssembly
                .CreatableTypes()
                .Where(type =>
                           type.Namespace?.Contains(".Queries") == true &&
                           type
                               .GetInterfaces()
                               .Any(@interface =>
                                        @interface.IsGenericType &&
                                        @interface.GetGenericTypeDefinition() == typeof(IQueryHandler<,>)))
                .AsInterfaces()
                .RegisterAsDynamic();

        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 10 — Dispatchers
        //
        // CommandDispatcher routes a command to its registered handler.
        // DomainEventDispatcher fires all registered handlers for a
        // domain event after a command completes.
        //
        // ══════════════════════════════════════════════════════════════════

#region Dipatchers

        private static void RegisterDispatchers( IMvxIoCProvider iocProvider )
        {
            iocProvider.RegisterType<IDomainEventDispatcher, AuditingDomainEventDispatcher>();


            iocProvider.RegisterSingleton(
                new CommandDispatcher(iocProvider));

            iocProvider.RegisterSingleton<IQueryExecutionScope>(
                new QueryExecutionScope(iocProvider.Resolve<ISqlConnectionFactory>()));

            iocProvider.RegisterSingleton(
                new QueryDispatcher(
                    iocProvider,
                    iocProvider.Resolve<IQueryExecutionScope>()));
        }

#endregion

        // ══════════════════════════════════════════════════════════════════
        // SECTION 11 — Core (Presentation) Services
        //
        // Infrastructure has no compile-time reference to Presentation.Core
        // or Presentation.Wpf — a reference in that direction would invert
        // the dependency graph (Infrastructure is supposed to depend on
        // nothing above it). Instead, this section is just a seam:
        // Infrastructure exposes "here's where presentation services get
        // registered" without knowing what they are. The one project that's
        // ALLOWED to know both Infrastructure and Presentation.Core — the
        // Wpf project's Setup — supplies the actual registrations through
        // this delegate. Same idea as passing an Assembly into
        // RegisterViewModels below instead of referencing the Wpf project
        // directly.
        // ══════════════════════════════════════════════════════════════════
        private static void RegisterCoreServices( IMvxIoCProvider iocProvider, Action<IMvxIoCProvider> registerPresentationServices )
        {
            // Project is yet to be created, so this is a placeholder for the future registration of ViewModels.
            registerPresentationServices(iocProvider);
        }

        // ══════════════════════════════════════════════════════════════════
        // SECTION 12 — ViewModels
        //
        // Scanned by convention from the WPF assembly. Any class ending
        // in "ViewModel" is registered as a dynamic type (created fresh
        // on every resolution) so each screen gets its own instance.
        //
        // Note: typeof(App).Assembly references the WPF project's assembly.
        // This requires a reference from Infrastructure to the WPF project,
        // which is a layering concern. The alternative is to call this
        // registration from App.xaml.cs itself and pass the assembly in
        // as a parameter — cleaner if you want strict layering.
        // ══════════════════════════════════════════════════════════════════
        private static void RegisterViewModels( IMvxIoCProvider iocProvider, Assembly viewModelAssembly )
        {
            // Project is yet to be created, so this is a placeholder for the future registration of ViewModels.
            viewModelAssembly
                .CreatableTypes()
                .EndingWith("ViewModel")
                .AsTypes()
                .RegisterAsDynamic();
        }

        // ══════════════════════════════════════════════════════════════════
        // PIPELINE BUILDER — private helper
        //
        // Constructs the decorator chain for a single command type:
        //
        //   LoggingCommandHandler
        //     AuthorizationCommandHandler
        //       ValidatingCommandHandler
        //         TransactionCommandHandler
        //           ActualCommandHandler   ← built by handlerFactory
        //
        // Registered as RegisterType (not singleton) so a fresh pipeline
        // is built for each command resolution — giving each command
        // execution its own UnitOfWork/transaction.
        //
        // Validators are resolved lazily from the container — if no
        // validator is registered for TCommand, ResolveValidators returns
        // an empty enumerable and validation is skipped cleanly.
        // ══════════════════════════════════════════════════════════════════

#region Command Handler Pipeline

        private static void BuildPipeline<TCommand, TResult>( IMvxIoCProvider iocProvider, Func<ICommandHandler<TCommand, TResult>> handlerFactory )
            where TCommand : ICommand<TResult>
        {
            iocProvider.RegisterType<ICommandHandler<TCommand, TResult>>(() =>
            {
                // ONE UnitOfWork for this pipeline, published ambiently for the
                // duration of this synchronous graph build only. Cleared in
                // finally before this factory returns — i.e. before
                // CommandDispatcher.SendAsync even begins awaiting HandleAsync —
                // so there's no risk of it leaking into a concurrent or nested
                // command's resolution.
                UnitOfWork unitOfWork = new(iocProvider.Resolve<ISqlConnectionFactory>());
                AmbientUnitOfWork.Current = unitOfWork;

                try
                {
                    // Layer 1 — innermost actual handler 
                    ICommandHandler<TCommand, TResult> inner = handlerFactory();

                    // Layer 2 — transaction, using the SAME unitOfWork instance
                    ICommandHandler<TCommand, TResult> transacted =
                        new TransactionCommandHandler<TCommand, TResult>(inner, unitOfWork);

                    // Layer 3 — validation
                    IEnumerable<IValidator<TCommand>> validators = ResolveValidators<TCommand>(iocProvider);

                    ICommandHandler<TCommand, TResult> validated =
                        new ValidatingCommandHandler<TCommand, TResult>(transacted, validators);

                    // Layer 4 - authorization
                    ICommandHandler<TCommand, TResult> authorized = new AuthorizationCommandHandler<TCommand, TResult>(
                        validated,
                        iocProvider.Resolve<IUserRepository>(),
                        iocProvider.Resolve<ICurrentUserContext>());

                    // Layer 5 — logging
                    ILogger<LoggingCommandHandler<TCommand, TResult>> logger =
                        iocProvider
                            .Resolve<ILoggerFactory>()
                            .CreateLogger<LoggingCommandHandler<TCommand, TResult>>();

                    return new LoggingCommandHandler<TCommand, TResult>(authorized, logger);
                }
                finally
                {
                    AmbientUnitOfWork.Current = null;
                }
            });
        }

        private static IEnumerable<IValidator<TCommand>> ResolveValidators<TCommand>( IMvxIoCProvider iocProvider ) =>
            iocProvider.TryResolve(out IEnumerable<IValidator<TCommand>> validators)
                ? validators
                : [];

#endregion
    }
}
