// -----------------------------------------------------------------------
// <copyright file="GreaterThanAttribute.cs" company="Inflexion Software">
//     Copyright (c) 2012. Inflexion Software. All Rights Reserved.
// </copyright>
// -----------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class of type attribute in charge of validating data within a range.
    /// </summary>
    public class GreaterThanAttribute : ValidationAttribute
    {
        #region Fields

        /// <summary>
        /// Variable privada que representa el nombre de la propiedad.
        /// </summary>
        /// <remarks>
        /// Sin comentarios adicionales.
        /// </remarks>
        private string basePropertyName;

        /// <summary>
        /// Variable privada encargada de almacenar el mensaje de error.
        /// </summary>
        /// <remarks>
        /// Sin comentarios adicionales.
        /// </remarks>
        private string errorMessage;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="T:GreaterThanAttribute"/>.
        /// </summary>
        /// <remarks>
        /// Sin comentarios adicionales.
        /// </remarks>
        /// <param name="basePropertyName">
        /// Parámetro que indica el nombre base de la propiedad de la que comparar.
        /// </param>
        /// <param name="errorMessage">
        /// Parámetro que indica el mensaje en caso de error.
        /// </param>
        public GreaterThanAttribute(
            string basePropertyName,
            string errorMessage)
        {
            this.basePropertyName = basePropertyName;
            this.errorMessage = errorMessage;
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Función encargada de validar si un número es mayor que otro.
        /// </summary>
        /// <param name="value">
        /// Parámetro que indica el número a comparar.
        /// </param>
        /// <param name="validationContext">
        /// Parámetro que indica el contexto de validación.
        /// </param>
        /// <returns>
        /// Devuelve <c>true</c> si el valor a comparar es mayor y <c>false</c> en caso contrario.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Obtenemos el objeto PropertyInfo.
            var basePropertyInfo = validationContext.ObjectType.GetProperty(this.basePropertyName);
            // Obtenemos el valor de la propiedad.
            int startData = (int)basePropertyInfo.GetValue(validationContext.ObjectInstance, null);
            // Obtenemos el valor de la fecha que nos viene para comparar.
            var thisData = (int)value;
            // Comprobamos los datos.
            if (thisData < startData)
            {
                // Devolvemos la lsita de errores de validación.
                return new ValidationResult(this.errorMessage, new List<string>()
                {
                    validationContext.MemberName
                });
            }
            // No ha habido error, devolvemos la respuesta.
            return null;
        }

        #endregion Methods
    }
}