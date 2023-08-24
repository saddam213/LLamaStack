namespace LLamaStack.Core.Services
{
    /// <summary>
    /// Structure to handle a Result and Error/Validation response
    /// </summary>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <typeparam name="TError">The type of the error.</typeparam>
    public readonly struct ServiceResult<TValue, TError>
    {
        private readonly TValue _value;
        private readonly TError _error;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResult{TValue, TError}"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        private ServiceResult(TValue value)
        {
            _value = value;
            _error = default;
            IsError = false;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceResult{TValue, TError}"/> struct.
        /// </summary>
        /// <param name="error">The error.</param>
        private ServiceResult(TError error)
        {
            _error = error;
            _value = default;
            IsError = true;
        }


        /// <summary>
        /// Gets a value indicating whether this instance is error.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is error; otherwise, <c>false</c>.
        /// </value>
        public bool IsError { get; }


        /// <summary>
        /// Gets a value indicating whether this instance is success.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is success; otherwise, <c>false</c>.
        /// </value>
        public bool IsSuccess => !IsError;

        /// <summary>
        /// Performs an implicit conversion from <see cref="TValue"/> to <see cref="ServiceResult{TValue, TError}"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ServiceResult<TValue, TError>(TValue value) => new(value);


        /// <summary>
        /// Performs an implicit conversion from <see cref="TError"/> to <see cref="ServiceResult{TValue, TError}"/>.
        /// </summary>
        /// <param name="error">The error.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ServiceResult<TValue, TError>(TError error) => new(error);


        /// <summary>
        /// Resolves the specified result as success or error.
        /// </summary>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="success">The success.</param>
        /// <param name="error">The error.</param>
        /// <returns></returns>
        public TResult Resolve<TResult>(Func<TValue, TResult> success, Func<TError, TResult> error) => !IsError ? success(_value) : error(_error);
    }

}
