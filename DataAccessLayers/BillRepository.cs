using BusinessObjects;

namespace DataAccessLayers
{
    public class BillRepository(Prn221projectContext context) : GenericRepository<Bill>(context)
    {
    }
}
