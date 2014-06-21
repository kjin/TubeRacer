using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace RibbonsRedux.Content
{
    /// <summary>
    /// A class containing text file information.
    /// </summary>
    public class Text
    {
        private string[] lines;
        /// <summary>
        /// Constructs a new Text instance.
        /// </summary>
        /// <param name="lines">The lines that make up the text file.</param>
        public Text(string[] lines) { this.lines = lines; }
        public string this[int i] { get { return lines[i]; } }
        /// <summary>
        /// Gets the number of lines in this text file.
        /// </summary>
        public int Length { get { return lines.Length; } }
    }

    public class TextReader : ContentTypeReader<Text>
    {
        protected override Text Read(ContentReader input, Text existingInstance)
        {
            List<String> lines = new List<String>();
            String str = "";
            try
            {
                str = input.ReadString();
            }
            catch
            {
                return new Text(new string[0]);
            }
            while (true)
            {
                lines.Add(str);
                try
                {
                    str = input.ReadString();
                }
                catch
                {
                    break;
                }
            }
            return new Text(lines.ToArray());
        }
    }
}
