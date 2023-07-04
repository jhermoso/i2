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


namespace Inflexion2.Domain
{
    using System;

    /// <summary>
    ///     Facilitates indicating which Classes are forbidden deleteable
    /// </summary>
    /// <remarks>
    ///     This is intended for use with <see cref="IEntity{TIdentifier}" />.  It may NOT be used on a <see cref="ValueObject" />.
    /// </remarks>
    [Serializable]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class ChildrenRelationshipDeleteBehaviorAttribute : Attribute
    {

        /// <summary>
        /// Discriminant value. allow only threee behaviors
        /// </summary>
        public readonly Delete Behavior;

        /// <summary>
        /// empty constructor set default value to cascade
        /// TODO: modificar este comportamiento para que sea cascade en composici�n y orphannull en agregaci�n.
        /// </summary>
        public ChildrenRelationshipDeleteBehaviorAttribute()
        {
            Behavior = Delete.Cascade; // Default Behavior
        }

        /// <summary>
        /// .es Constructor para establecer el comportamiento deseado
        /// .en constructor to set the behavieor
        /// </summary>
        /// <param name="attributeValue"></param>
        public ChildrenRelationshipDeleteBehaviorAttribute(Delete attributeValue)
        {
            this.Behavior = attributeValue; 
        }

    }

    /// <summary>
    /// .es lista de comportamientos validos 
    /// .en valid behavieors listing
    /// </summary>
    public enum Delete
    {
        /// <summary>
        /// .es con este comportamiento indicamos que al borrar la clase se borran todos las entidades hijas.
        /// Este comportamiento solo tiene sentido en relaciones de composici�n
        /// .en delete the children entitys 
        /// </summary>
        Cascade = 1,

        /// <summary>
        /// .es indicamos que no podemos borrar la entidad si esta relaci�n/propiedad tiene hijos. 
        /// Este comportamiento solo tiene sentido en relaciones de composici�n
        /// .en Can�t delete if there is at least one child entity or value object in this property/relationship
        /// </summary>
        Restrict = 2,

        ///// <summary>
        ///// .es indicamos que para esta propiedad/relaci�n podemos borrar la clase dejando huerfanos 
        ///// los hijos es decir que tenemos que asignar un valor nullo a la propiedad de cada hijo que
        ///// referencia al padre.
        ///// Este comportamiento solo tiene sentido en relaciones de agregaci�n y se adopta por defecto por lo que se ha eliminado.
        ///// </summary>
        ////OrphanNull = 3,  // you can delete and all the children will be orphans with their parent property seted to null.

        ///// <summary>
        ///// .es indicamos que para esta propiedad/relacion podemos borrar la clase sin cambiar la refere
        ///// </summary>
        ////Orphan = 4
    };
}