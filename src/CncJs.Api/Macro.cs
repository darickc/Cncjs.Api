using Cncjs.Api.Models;
using CSharpFunctionalExtensions;

namespace Cncjs.Api;

public class Macro
{
    private readonly CncJsClient _cncJs;

    private const string GetMacrosPath = "macros";

    public Macro(CncJsClient cncJs)
    {
        _cncJs = cncJs;
    }

    public Task<Result<MacroModel[]>> GetMacros()
    {
        return _cncJs.Get<MacroModel[]>(GetMacrosPath);
    }
}