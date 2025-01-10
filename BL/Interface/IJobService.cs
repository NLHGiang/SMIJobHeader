using SMIJobHeader.Model.Job;

namespace SMIJobHeader.BL.Interface;

public interface IJobService
{
    void RegisterJob(BatchJob jobConfig);
}