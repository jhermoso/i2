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
    using i2.Framework.Domain.Model.DynamicExtensions;
    using System;
    using System.Collections.Generic;

    /// <summary>
    ///  poc for CQRS implementation 
    /// </summary>
    [Serializable]
    public class Event
    {
        /// <summary>
        /// event version
        /// </summary>
        public int Version
        {
            get;
            set;
        }
    }

    /// <summary>
    /// event source entity to save historic changes
    /// </summary>
    public abstract class EventSourcedEntity
    {
        private readonly List<Event> _changes = new List<Event>();

        /// <summary>
        /// entity identifier
        /// </summary>
        public Guid Id
        {
            get;
            protected set;
        }

        /// <summary>
        /// TODO: update comments
        /// </summary>
        public int Version
        {
            get;
            internal set;
        }

        /// <summary>
        /// TODO: update comments
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Event> GetUncommittedChanges()
        {
            return this._changes;
        }

        /// <summary>
        /// TODO: update comments
        /// </summary>
        /// <param name="history"></param>
        public void LoadsFromHistory(IEnumerable<Event> history)
        {
            foreach (var e in history)
            {
                this.ApplyChange(e, false);
                this.Version = e.Version;
            }
        }

        /// <summary>
        /// TODO: update comments
        /// </summary>
        public void MarkChangesAsCommitted()
        {
            this._changes.Clear();
        }

        /// <summary>
        /// TODO: update comments
        /// </summary>
        /// <param name="event"></param>
        protected void ApplyChange(Event @event)
        {
            this.ApplyChange(@event, true);
        }

        private void ApplyChange(Event @event, bool isNew)
        {
            this.AsDynamic().Apply(@event);
            if (isNew)
            {
                this._changes.Add(@event);
            }
        }
    }
}