using SMIJobXml.Model.Job;

namespace SMIJobXml.BL.Interface
{
    public interface IJobService
    {
        void RegisterJob(BatchJob jobConfig);
    }
}
