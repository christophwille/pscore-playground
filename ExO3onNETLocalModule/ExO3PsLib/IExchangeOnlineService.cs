namespace ExO3PsLib;

public interface IExchangeOnlineService
{
    Task<ExOResult> GetExoMailbox();
    Task<ExOResult> GetExoMailboxWithPfx(ExOPfx pfxInfo);
    ExOPfx GetPfxInfo();
}
