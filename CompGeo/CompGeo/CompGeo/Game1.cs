using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CompGeo
{
    public class point
    {
        public int id;
        public bool alive;
        public int x;
        public int y;
        public int xdir;
        public int ydir;
    };
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        KeyboardState keyboardState;
        KeyboardState prevKeyboardState;
        StreamReader file;

        const string path = @"C:\Users\Nicholas\input.txt";

        List<point> points = new List<point>();
        int wait;
        int stepNum = 1;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            prevKeyboardState = Keyboard.GetState();
            file = File.OpenText(path);
            string lineOne = file.ReadLine();
            string[] lineOneSplit = lineOne.Split(null);
            for (int i = 0; i < lineOneSplit.Count(); i+=2)
            {
                point somePoint = new point();
                somePoint.id = Convert.ToInt32(lineOneSplit[i]);
                string[] coords = lineOneSplit[i+1].Split(',');
                somePoint.x = Convert.ToInt32(coords[0].Substring(1, coords[0].Length-1));
                somePoint.y = Convert.ToInt32(coords[1].Substring(0, coords[1].Length-1));
                somePoint.alive = true;
                Console.WriteLine(somePoint.id);
                points.Add(somePoint);
            }
            string lineTwo = file.ReadLine();
            string[] lineTwoSplit = lineTwo.Split(null);
            for (int i = 0; i < lineTwoSplit.Count(); i+=2)
            {
                int ndx = Convert.ToInt32(lineTwoSplit[i]);
                string[] dirs = lineTwoSplit[i+1].Split(',');
                points[ndx].xdir = Convert.ToInt32(dirs[0].Substring(1, dirs[0].Length-1));
                points[ndx].ydir = Convert.ToInt32(dirs[1].Substring(0, dirs[1].Length-1));
                Console.WriteLine("id: " + ndx + ", x: " + points[ndx].x + ", y: " + points[ndx].y + ", xdir: " + points[ndx].xdir + ", ydir: " + points[ndx].ydir);
            }
            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        void step()
        {
            Console.WriteLine("Step " + stepNum + ":");
            string currLine;
            if (wait > 0)
            {
                --wait;
                Console.WriteLine("Twiddle thumbs...");
            }
            else if ((currLine = file.ReadLine()) != null)
            {
                string[] currLineSplit = currLine.Split(null);
                if (currLineSplit[0] == "wait")
                {
                    wait = Convert.ToInt32(currLineSplit[1]);
                    --wait;
                    Console.WriteLine("Twiddle thumbs...");
                }
                else
                {
                    Console.Write("Changing ");
                    for (int i = 0; i < currLineSplit.Count(); i += 2)
                    {
                        int ndx = Convert.ToInt32(currLineSplit[i]);
                        Console.Write(ndx + " ");
                        string[] dirs = currLineSplit[i + 1].Split(',');
                        points[ndx].xdir = Convert.ToInt32(dirs[0].Substring(1, dirs[0].Length - 1));
                        points[ndx].ydir = Convert.ToInt32(dirs[1].Substring(0, dirs[1].Length - 1));
                    }
                    Console.WriteLine("");
                }
            }
            else
                Console.WriteLine("There is peace");
            for (int i = 0; i < points.Count(); ++i)
            {
                points[i].x += points[i].xdir;
                points[i].y += points[i].ydir;
                Console.WriteLine("id: " + i + ", x: " + points[i].x + ", y: " + points[i].y + ", xdir: " + points[i].xdir + ", ydir: " + points[i].ydir);
            }
            ++stepNum;
        }

        protected override void Update(GameTime gameTime)
        {
            keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Space) && !prevKeyboardState.IsKeyDown(Keys.Space))
            {
                step();
            }
            prevKeyboardState = keyboardState;
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            base.Draw(gameTime);
        }
    }
}
