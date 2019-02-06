using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeamStor.AOD.Gameplay
{
    /// <summary>
    /// Event stored in a save to be restored at a later date.
    /// This will be loaded and saved in chronological order.
    /// Loading occurs by saving the type name of the event and then later creating
    /// a new instance with reflection. If the specified type doesn't exist
    /// the save event will be ignored.
    /// </summary>
    public abstract class SaveRecord
    {
        /// <summary>
        /// Time when this event occured (UTC).
        /// </summary>
        public DateTime TimeOfEvent { get; private set; }

        public SaveRecord()
        {
            TimeOfEvent = DateTime.UtcNow;
        }

        public SaveRecord(DateTime timeOfEvent, JsonReader save)
        {
            TimeOfEvent = timeOfEvent;

            // TODO: read your parts of the save here.
        }

        /// <summary>
        /// Writes the data of this event to a save.
        /// </summary>
        /// <param name="writer">The JSON writer the save uses.</param>
        public abstract void WriteToSave(JsonWriter writer);
    }
}
