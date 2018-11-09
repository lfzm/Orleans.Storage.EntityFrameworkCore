using System;
using System.Runtime.Serialization;

namespace Orleans.Storage.EntityFrameworkCore
{
    public class RepositoryException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="RepositoryDataException"/> object.
        /// </summary>
        public RepositoryException()
        {

        }

        /// <summary>
        /// Creates a new <see cref="RepositoryDataException"/> object.
        /// </summary>
        public RepositoryException(SerializationInfo serializationInfo, StreamingContext context)
            : base(serializationInfo, context)
        {

        }

        /// <summary>
        /// Creates a new <see cref="RepositoryDataException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        public RepositoryException(string message)
            : base(message)
        {

        }

        /// <summary>
        /// Creates a new <see cref="RepositoryDataException"/> object.
        /// </summary>
        /// <param name="message">Exception message</param>
        /// <param name="innerException">Inner exception</param>
        public RepositoryException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
