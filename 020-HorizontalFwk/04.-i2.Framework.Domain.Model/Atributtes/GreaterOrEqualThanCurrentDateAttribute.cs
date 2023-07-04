// ----------------------------------------------------------------------------------------
// <copyright file="GreaterOrEqualThanCurrentDateAttribute.cs" company="Inflexion Software">
//     Copyright (c) 2012. Inflexion Software. All Rights Reserved.
// </copyright>
// ----------------------------------------------------------------------------------------
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
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class attribute in charge of checking if a date is less than the current date.
    /// </summary>
    public class GreaterOrEqualThanCurrentDateAttribute : ValidationAttribute
    {
        #region Fields

        /// <summary>
        /// Variable encargada de almacenar el mensaje de error.
        /// </summary>
        private string errorMessage = "TODO: escribir mensaje de error y pasarlo a recursos";
        
        #endregion Fields

        #region Constructors

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="T:GreaterOrEqualThanCurrentDateAttribute"/>.
        /// </summary>
        /// <remarks>
        /// Sin comentarios adicionales.
        /// </remarks>
        public GreaterOrEqualThanCurrentDateAttribute()
        {
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Función encargada de validar si una fecha es mayor a otra.
        /// </summary>
        /// <param name="value">
        /// Parámetro que indica la fecha a comparar.
        /// </param>
        /// <param name="validationContext">
        /// Parámetro que indica el contexto de validación.
        /// </param>
        /// <returns>
        /// Devuelve <c>true</c> si el valor de la fecha es válido y <c>false</c> en caso contrario.
        /// </returns>
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Obtenemos el valor de la fecha que nos viene para comparar.
            var thisDate = (DateTime)value;
            // Comprobamos las fechas.
            if (DateTime.Compare(thisDate, DateTime.Now) < 0)
            {
                // Devolvemos los errores.
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