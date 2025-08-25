using ZetaDashboard.Common.Mongo.DataModels;
using ZetaDashboard.Common.Http;

namespace ZetaDashboard.Common.ZDB.Services
{
    public partial class BaseService
    {
        public BaseService(MongoContext context)
        {
            _context = context;

            // Inicializas repositorios por colección
            Users = new UserService(context);
            Proyects = new ProyectService(context);
            Audits = new AuditService(context);
            Notes = new NoteService(context);
            Plans = new PlanListService(context);
            SeenMovies = new SeenMovieService(context);
        }

        private readonly MongoContext _context;
        private readonly HttpClient? _http;

        public UserService Users { get; }
        public ProyectService Proyects { get; }
        public AuditService Audits { get; }
        public NoteService Notes { get; }
        public PlanListService Plans { get; }
        public SeenMovieService SeenMovies { get; }
        //public MovieService Movies { get; }
    }
}
