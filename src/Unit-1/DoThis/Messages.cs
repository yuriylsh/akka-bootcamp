namespace WinTail
{
    public static class Messages
    {
        public static readonly Error.NullInputError NoInputMessage =
            new Error.NullInputError("No input received.");

        public static readonly Success.InputSuccess InputSuccessMessage = 
            new Success.InputSuccess("Thank you! Message was valid.");

        public static readonly Error.ValidationError ValidationErrorMessage = 
            new Error.ValidationError("Invalid: input had odd number of characters.");

        public static readonly Neutral.ContinueProcessing ContinueProcessingMessage = 
            new Neutral.ContinueProcessing();

        public class Neutral
        {
            public class ContinueProcessing{}
        }

        public class Success
        {
            public class InputSuccess
            {
                public InputSuccess(string reason) => Reason = reason;
                public string Reason {get;}
            }
        }

        public class Error
        {
            public class InputError
            {
                public InputError(string reason) => Reason = reason;
                public string Reason { get; }
            }

            public class NullInputError : InputError
            {
                public NullInputError(string reason) : base(reason) { }
            }

            public class ValidationError : InputError
            {
                public ValidationError(string reason) : base(reason) { }
            }
        }
    }
}