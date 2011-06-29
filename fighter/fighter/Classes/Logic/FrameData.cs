using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.Xna.Framework;

namespace fighter
{
    public class FrameData
    {
        public FrameData(string CharName)
        {
            charName = CharName;

            XmlDocument doc = new XmlDocument();
            doc.Load(constants.SETTINGS_FILENAME);

            foreach (XmlNode node in doc["characters"].ChildNodes)
            {
                if (String.Compare(node.FirstChild.InnerText, charName) != 0)
                    continue;
                else
                {
                    int i;

                    string[] parseMe = node["animationOffsets"].InnerText.Split('|');
                    animationOffsets = new Vector2[parseMe.Length];
                    for (i = 0; i < parseMe.Length; i++)
                    {
                        string[] splitAgain = parseMe[i].Split(',');
                        animationOffsets[i] = new Vector2(Convert.ToInt32(splitAgain[0]), Convert.ToInt32(splitAgain[1]));
                    }

                    parseMe = node["animationFrames"].InnerText.Split(',');
                    animationFrames = new int[parseMe.Length];
                    for (i = 0; i < parseMe.Length; i++)
                    {
                        animationFrames[i] = Convert.ToInt32(parseMe[i]);
                    }

                    parseMe = node["damage"].InnerText.Split(',');
                    damageValues = new int[parseMe.Length];
                    for (i = 0; i < parseMe.Length; i++)
                    {
                        damageValues[i] = Convert.ToInt32(parseMe[i]);
                    }

                    parseMe = node["pushback"].InnerText.Split(',');
                    pushBack = new int[parseMe.Length];
                    for (i = 0; i < parseMe.Length; i++)
                    {
                        pushBack[i] = Convert.ToInt32(parseMe[i]);
                    }

                    parseMe = node["Bpushback"].InnerText.Split(',');
                    blockedPushback = new int[parseMe.Length];
                    for (i = 0; i < parseMe.Length; i++)
                    {
                        blockedPushback[i] = Convert.ToInt32(parseMe[i]);
                    }

                    parseMe = node["activeAnimationFrames"].InnerText.Split('|');
                    activeAnimationFrames = new List<List<int>>(parseMe.Length);
                    for (i = 0; i < parseMe.Length; i++)
                    {
                        string[] splitAgain = parseMe[i].Split(',');
                        foreach (String activeFrame in splitAgain)
                        {
                            if (!activeFrame.Equals(""))
                            {
                                activeAnimationFrames.Insert(i, new List<int>());
                                activeAnimationFrames[i].Add(Convert.ToInt32(activeFrame));
                            }
                            else
                            {
                                activeAnimationFrames.Insert(i, null);
                            }
                        }
                    }
                }
            }
        }
        public string charName;
        public Vector2[] animationOffsets;
        public int[] animationFrames;
        public int[] damageValues;

        public int[] pushBack;
        public int[] blockedPushback;
        public List<List<int>> activeAnimationFrames;
    }
}
