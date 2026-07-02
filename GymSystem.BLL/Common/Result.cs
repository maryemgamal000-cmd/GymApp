using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.BLL.Common
{
    public sealed record Result (bool success , string? error = null , ResultKind kind = ResultKind.Ok)
    {
        public static Result Ok() => new Result(true);

        public static Result Fail(string error, ResultKind kind = ResultKind.Conflict) => new Result(false, error, kind);

        public static Result NotFound(string error = "Not Found" ) => new Result(false , error , ResultKind.NotFound);

        public static Result Validation (string error) => new Result(false , error , ResultKind.ValidationFailed);  
    }


    public sealed record Result<T> (bool success , T? value , string? error = null , ResultKind kind = ResultKind.Ok )
    {
        public static Result<T> Ok(T value) => new Result<T>(true, value);
        public static Result<T> Fail(string error, ResultKind kind = ResultKind.Conflict) => new Result<T>(false, default, error, kind);
        public static Result<T> NotFound(string error = "Not Found") => new Result<T>(false, default, error, ResultKind.NotFound);
    }
}
