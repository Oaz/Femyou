
namespace Femyou
{
  public enum Status
  {
    Ok,
    Warning,
    Discard,
    Error,
    Fatal,
    Pending
};
  public interface ICallbacks
  {
    void Logger(IInstance instance, Status status, string category, string message);
  }
}