using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Tethys.Server.Services
{
    public interface IFileUploadManager
    {
        Task LoadSequenceFromStream(IEnumerable<Stream> streams);
    }
}