// -------------------------------------------------------------------------------
// <copyright file="MinimumCollectionSizeAttribute.cs" company="Inflexion Software">
//     Copyright (c) 2012. Inflexion Software. All Rights Reserved.
// </copyright>
// -------------------------------------------------------------------------------
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

namespace i2.Framework.Domain.Model.Domain
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Attribute class for validating the minimum size of a collection.
    /// </summary>
    /// <remarks>
    /// We validate the minimum number of elements that the collection must have.
    /// </remarks>
    public class MinimumCollectionSizeAttribute : ValidationAttribute
    {
        #region Fields

        /// <summary>
        ///  Variable privada que indica el tamañon mínimo de la colección.
        /// </summary>
        /// <remarks>
        /// Sin comentarios adicionales.
        /// </remarks>
        private int minSize;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Inicializa una nueva instancia de la clase MinimumCollectionSizeAttribute.
        /// </summary>
        /// <remarks>
        /// Sin comentarios adicionales.
        /// </remarks>
        /// <param name="minSize">
        /// Parámetro que indica el tamaño mínimo.
        /// </param>
        public MinimumCollectionSizeAttribute(int minSize)
        {
            this.minSize = minSize;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Función para validar el tamaño mínimo de una colección.
        /// </summary>
        /// <param name="value">
        /// Parámetro que corresponde a la colección que se quiere validar.
        /// </param>
        /// <returns>
        /// Devuelve <c>true</c> sin la colección cumple con el tamaño mínimo y <c>false</c>
        /// en caso contrario.
        /// </returns>
        public override bool IsValid(object value)
        {
            // Comprobamos el argumento de entrada.
            if (value == null)
            {
                return true;
            }
            // Cast de la colección.
            var list = value as System.Collections.ICollection;
            // Comprobamos si es una lista.
            if (list == null)
            {
                return true;
            }
            // Devolvemos la respueta.
            return list.Count >= minSize;
        }

        #endregion Methods
    }
}