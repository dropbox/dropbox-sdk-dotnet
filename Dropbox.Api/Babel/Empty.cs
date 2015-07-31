namespace Dropbox.Api.Babel
{
    /// <summary>
    /// An empty object used when a route doesn't have one or more of the
    /// request, response, or error types specified.
    /// </summary>
    public sealed class Empty : IEncodable<Empty>
    {
        /// <summary>
        /// A static instance of the <see cref="Empty"/> class.
        /// </summary>
        public static readonly Empty Instance = new Empty();

        /// <summary>
        /// Encodes the object using the supplied encoder.
        /// </summary>
        /// <param name="encoder">The encoder being used to serialize the object.</param>
        public void Encode(IEncoder encoder)
        {
        }

        /// <summary>
        /// Decodes on object using the supplied decoder.
        /// </summary>
        /// <param name="decoder">The decoder used to deserialize the object.</param>
        /// <returns>
        /// The deserialized object. Note: this is not necessarily the current instance.
        /// </returns>
        public Empty Decode(IDecoder decoder)
        {
            return this;
        }
    }
}
