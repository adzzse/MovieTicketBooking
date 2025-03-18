using BusinessObjects;

namespace DataAccessLayers
{
    public class BillRepository(MovieprojectContext context) : GenericRepository<Bill>(context)
    {
    }
}
