/* 
 * Copyright [2023] [Javier Hermoso Blanco]
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *     http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

namespace i2.Framework.Domain.Contracts
{
    /// <summary>
    /// Interface for basic entity from Eric Evans definition.
    /// </summary>
    /// <typeparam name="TIdentifier">
    /// Representation of Entity's identifier.
    /// </typeparam>
    public interface IEntity<TIdentifier> : IDomainObject, System.IComparable,
                                            System.IEquatable<IEntity<TIdentifier>>, 
                                            System.IComparable<IEntity<TIdentifier>>
        where TIdentifier : System.IEquatable<TIdentifier>, System.IComparable<TIdentifier>
    {
        #region Properties

        /// <summary>
        /// .en Get unique entity's identification
        /// </summary>
        /// <remarks>
        /// The value of the unique identifier will be used as the main criterion during equality and comparison between entities.
        /// Main criterion during equality and comparison between entities.
        /// </remarks>
        /// <value>
        /// <para>
        /// unique entity identifier
        /// </para>
        /// <para>
        /// TIdentifier represents identity's type used to identify an entity.
        /// </para>
        /// </value>
        TIdentifier Id
        {
            get;
        }

        #endregion

        #region methods

        /// <summary>
        /// Transient objects are not associated with an item already in storage.  For instance,
        /// a Customer is transient if its Id is defalut value.  It's virtual to allow NHibernate-backed 
        /// objects to be lazily loaded.
        /// </summary>
        bool IsTransient();

        #endregion
    }
}