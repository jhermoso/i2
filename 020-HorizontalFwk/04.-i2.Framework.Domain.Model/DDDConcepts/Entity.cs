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

namespace i2.Framework.Domain.Model
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using i2.Framework.Domain.Contracts;

    /// <summary>
    /// Abstract public class to represent the business entities.
    /// </summary>
    /// <remarks>
    /// The abstract class <see cref="IEntity{TIdentifier}"/> represents a base class for the business entity type.
    /// </remarks>
    /// <typeparam name="TEntity">
    /// Generic type parameter to represent the actual type of the identifier.
    /// Its inclusion is justified to facilitate reflection operations,
    /// specifically to facilitate the construction of the CanDelete method in the AggregateRoot.
    /// </typeparam>
    /// <typeparam name="TIdentifier">
    /// Generic type parameter to represent the type of identifier for the entities,
    /// which is necessary for the repositories.
    /// </typeparam>
    /// <seealso cref="T:Inflexion2.Domain.IEntity{TIdentifier}" />
    [Serializable]
    public abstract class Entity<TEntity, TIdentifier> : IEntity<TIdentifier>
        where TEntity : IEntity<TIdentifier>, IEquatable<TEntity>, IComparable<TEntity>
        where TIdentifier : System.IEquatable<TIdentifier>, System.IComparable<TIdentifier>
    {
        #region Constants

        /// <summary>
        ///     To help ensure hashcode uniqueness, a carefully selected random number multiplier 
        ///     is used within the calculation.  Goodrich and Tamassia's Data Structures and
        ///     Algorithms in Java asserts that 31, 33, 37, 39, and 41 will produce the fewest number
        ///     of collisions.  See http://computinglife.wordpress.com/2008/11/20/why-do-hash-functions-use-prime-numbers/
        ///     for more information.
        /// </summary>
        static private readonly int HASH_MULTIPLIER = 31;

        #endregion

        #region Fields

        /// <summary>
        /// Unique identifier of the entity.
        /// </summary>
        /// <remarks>
        /// This field or variable is used in conjunction with the property
        /// <see cref="Id"/>.
        /// </remarks>
        private TIdentifier id;

        /// <summary>
        /// 
        /// </summary>
        private int? cachedHashcode;

        /// <summary>
        ///     This static member caches the domain signature properties to avoid looking them up for 
        ///     each instance of the same type.
        /// 
        ///     A description of the very slick ThreadStatic attribute may be found at 
        ///     http://www.dotnetjunkies.com/WebLog/chris.taylor/archive/2005/08/18/132026.aspx
        /// </summary>
        [ThreadStatic]
        private static Dictionary<Type, IEnumerable<PropertyInfo>> signaturePropertiesDictionary;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the unique identifier of the entity.
        /// </summary>
        /// <value>
        /// The unique identifier of the entity.
        /// </value>
        /// <remarks>
        /// <para>
        /// The value of the unique identifier will be used as
        /// the primary criterion for equality and comparison between
        /// entities.
        /// </para>
        /// <para>
        /// TIdentifier represents the data type of the unique identifier
        /// of the entity.
        /// </para>
        /// </remarks>
        public virtual TIdentifier Id
        {
            get
            {
                return this.id;
            }
            protected set
            {
                this.id = value;
            }
        }

        /// <summary>
        /// Gets the actual type of the entity, regardless of the level
        /// in the class hierarchy.
        /// </summary>
        /// <remarks>
        /// The actual type will be used as the primary criterion
        /// for equality and comparison between entities.
        /// </remarks>
        /// <value>
        /// The actual type (leaf type <see cref="T:System.Type"/>) of the
        /// entity.
        /// </value>
        public virtual System.Type ActualType
        {
            get
            {
                return typeof(Entity<TEntity, TIdentifier>);
            }
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Constructor of the class.
        /// </summary>
        /// <remarks>
        /// The constructor allows us to create an entity based on
        /// the unique identifier.
        /// </remarks>
        /// <param name="id">
        /// The unique identifier of the entity.
        /// </param>
        protected Entity(TIdentifier id)//:base()
        {
            this.Id = id;
        }

        /// <summary>
        /// Empty constructor of the class.
        /// </summary>
        /// <remarks>
        /// The constructor allows us to create an entity based on
        /// the unique identifier.
        /// </remarks>
        protected Entity()
        {

        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Defines the function responsible for comparing or ordering objects.
        /// </summary>
        /// <remarks>
        /// Compares the unique identifier of two entities to determine if
        /// they are equal or not.
        /// </remarks>
        /// <param name="element">
        /// The parameter that refers to the element to compare.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the value of the argument <c>element</c> is null.
        /// </exception>
        /// <exception cref="System.ArgumentException">
        /// Thrown when the value of the variable <c>otherEntity</c> is null.
        /// </exception>
        /// <returns>
        /// Returns an integer indicating whether the comparison is correct or not.
        /// </returns>
        public virtual int CompareTo(object element)
        {
            // Check that the element is not null.
            if (element == null)
            {
                throw new System.ArgumentNullException("element");
            }
            else
            {
                // Cast the argument.
                var otherEntity = element as IEntity<TIdentifier>;
                if (otherEntity == null)
                {
                    throw new System.ArgumentNullException("element");
                }
                else
                {
                    return this.CompareTo(otherEntity);
                }
            }
        }

        /// <summary>
        /// Defines the function responsible for comparing or ordering objects.
        /// </summary>
        /// <remarks>
        /// Compares the unique identifier of two entities to determine if
        /// they are equal or not.
        /// </remarks>
        /// <param name="entityIdentifier">
        /// The parameter that refers to the identifier to compare.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// Thrown when the value of the argument <c>entityIdentifier</c> is null.
        /// </exception>
        /// <returns>
        /// Returns an integer indicating whether the comparison is correct or not.
        /// </returns>
        public virtual int CompareTo(IEntity<TIdentifier> entityIdentifier)
        {
            if (entityIdentifier == null)
            {
                throw new System.ArgumentNullException("entityIdentifier");
            }
            else
            {
                // Use the id property as the primary sorting criterion.
                return this.Id.CompareTo(entityIdentifier.Id);
            }
        }

        /// <summary>
        /// Defines the function responsible for comparing or ordering objects.
        /// </summary>
        /// <remarks>
        /// Compares the identifier of two base entities to determine if
        /// they are equal or not.
        /// </remarks>
        /// <param name="entityIdentifier">Indicates the other object to compare with.</param>
        /// <returns>Returns an integer indicating whether the comparison is correct or not.</returns>
        public virtual int CompareTo(Entity<TEntity, TIdentifier> entityIdentifier)
        {
            return this.CompareTo(entityIdentifier as IEntity<TIdentifier>);
        }

        /// <summary>
        /// Defines the function responsible for comparing or ordering objects.
        /// </summary>
        /// <remarks>
        /// Compares the identifier of two base entities to determine if
        /// they are equal or not.
        /// </remarks>
        /// <param name="entity">Indicates the other object to compare with.</param>
        /// <returns>Returns an integer indicating whether the comparison is correct or not.</returns>
        public virtual int CompareTo(TEntity entity)
        {
            return this.CompareTo(entity as IEntity<TIdentifier>);
        }

        /// <summary>
        ///     Transient objects are not associated with an item already in storage.  For instance,
        ///     a Customer is transient if its Id is 0.  It's virtual to allow NHibernate-backed 
        ///     objects to be lazily loaded.
        /// </summary>
        public virtual bool IsTransient()
        {
            if (this.id == null && default(TIdentifier) == null)
            {
                return true;
            }
            return this.id.Equals(default(TIdentifier));
        }

        /// <summary>
        /// Equalses the specified compare to.
        /// </summary>
        /// <param name="other">The compare to.</param>
        /// <returns></returns>
        public virtual bool Equals(TEntity other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// Equalses the specified compare to.
        /// </summary>
        /// <param name="other">The compare to.</param>
        /// <returns></returns>
        public virtual bool Equals(IEntity<TIdentifier> other)
        {
            return this.Equals(other as IEntity<TIdentifier>);
        }

        /// <summary>
        /// Equalses the specified compare to.
        /// </summary>
        /// <param name="other">The compare to.</param>
        /// <returns></returns>
        public virtual bool Equals(Entity<TEntity, TIdentifier> other)
        {
            return base.Equals(other);
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">
        /// The <paramref name="obj"/> parameter is null.
        /// </exception>
        public override bool Equals(object obj)
        {
            var compareTo = obj as Entity<TEntity, TIdentifier>;

            if (object.ReferenceEquals(this, compareTo))
            {
                return true;
            }

            if (compareTo == null || compareTo is Entity<TEntity, TIdentifier> == false)
            {
                return false;
            }

            if (this.IsTransient())
            {
                return false;
            }

            return HasSameNonDefaultIdAs(compareTo);

            // Since the Ids aren't the same, both of them must be transient to
            // compare domain signatures; because if one is transient and the
            // other is a persisted entity, then they cannot be the same object.
            //return this.IsTransient() && compareTo.IsTransient();
        }

        /// <summary>
        /// This is used to provide the hashcode identifier of an object using the signature
        /// properties of the object; although it's necessary for NHibernate's use, this can
        /// also be useful for business logic purposes and has been included in this base
        /// class, accordingly.  Since it is recommended that GetHashCode change infrequently,
        /// if at all, in an object's lifetime, it's important that properties are carefully
        /// selected which truly represent the signature of an object.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Once we have a hash code, we'll never change it
            if (this.cachedHashcode.HasValue)
            {
                return this.cachedHashcode.Value;
            }

            if (IsTransient())
            {
                this.cachedHashcode = base.GetHashCode();
            }
            else
            {
                unchecked
                {
                    //var signatureProperties = this.GetSignatureProperties();  // Option 2 from SharpArchitecture                  

                    // It's possible for two objects to return the same hash code based on
                    // identically valued properties, even if they're of two different types,
                    // so we include the object's type in the hash calculation
                    int hashCode = this.GetType().GetHashCode();

                    // Option 1 
                    this.cachedHashcode = (hashCode * HASH_MULTIPLIER) ^ this.Id.GetHashCode();
                    // Option 2 from SharpArchitecture

                    //this.cachedHashcode = signatureProperties.Select(property => property.GetValue(this, null))
                    //                          .Where(value => value != null)
                    //                          .Aggregate(hashCode, (current, value) => (current * HASH_MULTIPLIER) ^ value.GetHashCode());

                    //if (signatureProperties.Any())
                    //{
                    //    return hashCode;
                    //}

                }
            }

            return cachedHashcode.Value;
        }

        /// <summary>
        /// Init the signaturePropertiesDictionary here due to reasons described at 
        /// http://blogs.msdn.com/jfoscoding/archive/2006/07/18/670497.aspx
        /// </summary>
        public virtual IEnumerable<PropertyInfo> GetSignatureProperties()
        {
            IEnumerable<PropertyInfo> properties;

            if (signaturePropertiesDictionary == null)
            {
                signaturePropertiesDictionary = new Dictionary<Type, IEnumerable<PropertyInfo>>();
            }

            if (signaturePropertiesDictionary.TryGetValue(this.GetType(), out properties))
            {
                return properties;
            }

            return signaturePropertiesDictionary[this.GetType()] = this.GetTypeSpecificSignatureProperties();
        }



        /// <summary>
        ///     When NHibernate proxies objects, it masks the type of the actual entity object.
        ///     This wrapper burrows into the proxied object to get its actual type.
        /// 
        ///     Although this assumes NHibernate is being used, it doesn't require any NHibernate
        ///     related dependencies and has no bad side effects if NHibernate isn't being used.
        /// 
        ///     Related discussion is at http://groups.google.com/group/sharp-architecture/browse_thread/thread/ddd05f9baede023a ...thanks Jay Oliver!
        /// </summary>
        protected virtual Type GetTypeUnproxied()
        {
            return this.GetType();
        }

        /// <summary>
        /// Returns true if self and the provided entity have the same Id values
        /// and the Ids are not of the default Id value
        /// </summary>
        private bool HasSameNonDefaultIdAs(Entity<TEntity, TIdentifier> compareTo)
        {
            return !this.IsTransient() &&
                   !compareTo.IsTransient() &&
                   this.Id.Equals(compareTo.Id);
        }

        /// <summary>
        ///     The property getter for SignatureProperties should ONLY compare the properties which make up 
        ///     the "domain signature" of the object.
        /// 
        ///     If you choose NOT to override this method (which will be the most common scenario), 
        ///     then you should decorate the appropriate property(s) with [DomainSignature] and they 
        ///     will be compared automatically.  This is the preferred method of managing the domain
        ///     signature of entity objects.
        /// </summary>
        /// <remarks>
        ///     This ensures that the entity has at least one property decorated with the 
        ///     [DomainSignature] attribute.
        /// </remarks>
        protected virtual IEnumerable<PropertyInfo> GetTypeSpecificSignatureProperties()
        {
            return
                this.GetType().GetProperties().Where(
                    p => Attribute.IsDefined(p, typeof(DomainSignatureAttribute), true));
        }

        #endregion
    }
}

