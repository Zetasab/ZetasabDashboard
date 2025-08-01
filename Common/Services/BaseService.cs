using ZetaDashboard.Common.Mongo.DataModels;
using static ZetaDashboard.Common.Mongo.DataModels.MongoBase;

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
        }

        private readonly MongoContext _context;

        public UserService Users { get; }
        public ProyectService Proyects { get; }
        public AuditService Audits { get; }
        public NoteService Notes { get; }
    }
}
