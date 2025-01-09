using SMIJobXml.Model.Job;

namespace SMIJobXml.RestAPI.Interface
{
    public interface IETLService
    {
        Task<T> Synchronized<T>(BatchJob modelDto);
    }
}
