using SMIJobHeader.Model.Job;

namespace SMIJobHeader.RestAPI.Interface;

public interface IETLService
{
    Task<T> Synchronized<T>(BatchJob modelDto);
}