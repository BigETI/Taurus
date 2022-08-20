/// <summary>
/// Taurus validators namespace
/// </summary>
namespace Taurus.Validators
{
    /// <summary>
    /// Used to invoke when an input needs to be validated
    /// </summary>
    /// <typeparam name="TInput">Input type</typeparam>
    /// <param name="input">Input</param>
    /// <returns>"true" if input is valid, otherwise "false"</returns>
    public delegate bool ValidateDelegate<TInput>(TInput input);
}
