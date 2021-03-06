﻿using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace wordsearch
{
    /*
    This class will perform the game logic in the backend
    */

    class Game
    {
        //board is 15x15
        private const int ROWS = 15;
        private const int COLS = 15;

        //global static board array, yes its bad practice but its convienent 
        public static string[,] wordsearchBoard = new String[ROWS, COLS];
        //dynamic array of chars for deck of Tiles
        public static List<string> DeckOfTiles = new List<string>();
        //computers hand of seven tiles
        public static List<string> ComputerHand = new List<string>();
        //list of cooordinate pairs for the computer
        public static List<KeyValuePair<int, int>> ComputerCoordinatePairs = new List<KeyValuePair<int, int>>();
        //sets up a Dictionary for score lookups
        public static Dictionary<string, int> ScoreTable = new Dictionary<string, int>();

        //Holds words defined in text file
        public static Dictionary<string, int> wordList; 

        public Game()
        {
            //at the start of the application read in all the words into dictionary
            SearchWord.MakeDictionary();
            //starts a new game
            NewGame();
        }

        //intitalizes backend board to blanks and setups the 100 tiles
        //sets up player hand and computers hand
        public void NewGame()
        {
            
            for (int i=0; i<ROWS; i++)
            {
                for(int j=0; j<COLS; j++)
                {
                    //intitalizing board to default blank chars
                    wordsearchBoard[i,j] = " ";
                }
            }      
            ScoreTable[" "] = 0;
            ScoreTable["A"] = 1;
            ScoreTable["B"] = 3;
            ScoreTable["C"] = 3;
            ScoreTable["D"] = 2;
            ScoreTable["E"] = 1;
            ScoreTable["F"] = 4;
            ScoreTable["G"] = 2;
            ScoreTable["H"] = 4;
            ScoreTable["I"] = 1;
            ScoreTable["J"] = 8;
            ScoreTable["K"] = 5;
            ScoreTable["L"] = 1;
            ScoreTable["M"] = 3;
            ScoreTable["N"] = 1;
            ScoreTable["O"] = 1;
            ScoreTable["P"] = 3;
            ScoreTable["Q"] = 10;
            ScoreTable["R"] = 1;
            ScoreTable["S"] = 1;
            ScoreTable["T"] = 1;
            ScoreTable["U"] = 1;
            ScoreTable["V"] = 4;
            ScoreTable["W"] = 4;
            ScoreTable["X"] = 8;
            ScoreTable["Y"] = 4;
            ScoreTable["Z"] = 10;

            //adding letter frequency into deck of Tiles. Total is 100 tiles.
            DeckOfTiles.Add("K");
            DeckOfTiles.Add("J");
            DeckOfTiles.Add("X");
            DeckOfTiles.Add("Q");
            DeckOfTiles.Add("Z");
            
            for (int i=0; i < 2; i++)
            {
                //two blank(wild) tiles are added which are worth zero points
                DeckOfTiles.Add(" ");
                DeckOfTiles.Add("B");
                DeckOfTiles.Add("C");
                DeckOfTiles.Add("M");
                DeckOfTiles.Add("P");

                DeckOfTiles.Add("F");
                DeckOfTiles.Add("H");
                DeckOfTiles.Add("V");
                DeckOfTiles.Add("W");
                DeckOfTiles.Add("Y");
            }

            for (int i = 0; i < 3; i++)
            {
                DeckOfTiles.Add("G");
            }

            for (int i = 0; i < 4; i++)
            {
                DeckOfTiles.Add("L");
                DeckOfTiles.Add("S");
                DeckOfTiles.Add("U");
                DeckOfTiles.Add("D");
            }

            for (int i = 0; i < 6; i++)
            {

                DeckOfTiles.Add("N");
                DeckOfTiles.Add("R");
                DeckOfTiles.Add("T");
            }
            for (int i = 0; i < 8; i++)
            {
                DeckOfTiles.Add("O");
            }
            for (int i = 0; i < 9; i++)
            {
                DeckOfTiles.Add("A");
                DeckOfTiles.Add("I");
            }

            for (int i = 0; i < 12; i++)
            {
                DeckOfTiles.Add("E");
                
            }
           
            //shuffles deck
            ShuffleTiles();
            //used for testing purpose only
            PrintDeck();
            //loads seven tiles of computer, playe hand will only be stored on front end
            SetupComputerHand();
        }
        //function used to randomly select a player
        public string DrawLetter()
        {
            Random rand = new Random();
            return ((char)('A' + rand.Next(0, 26))).ToString(); //Returns random letter A-Z
        }

        public string WhoseTurn(string yourLetter, string theirLetter)
        {
            if (String.Compare(yourLetter, theirLetter) == -1) //Compares the first two drawn letters to see who goes first
                return "Your Turn";

            return "Their Turn";            
        }

        //will randomize the deck of Tiles
        public void ShuffleTiles()
        {
            Random rand = new Random();
            //generates a random number in between 0 and 99 
            
            string temp;

            //randomly pick two values and make 200 swaps to simulate shuffling
            for(int i=0; i<200; i++)
            {
                int value1 = rand.Next(100);
                int value2 = rand.Next(100);

                temp = DeckOfTiles[value1];
                DeckOfTiles[value1] = DeckOfTiles[value2];
                DeckOfTiles[value2] = temp; 
            }
        }
        //used to draw a random tile from the Tile deck
        public static string DrawTile()
        {
            if(DeckOfTiles.Count == 0)
            {
                //deckOfTiles is empty so game will end with this function call
                GameOver();
                //blank junk tile is returned
                return " ";         
            }
            string temp = DeckOfTiles[DeckOfTiles.Count - 1];
            //deletes the last tile in the deck
            DeckOfTiles.RemoveAt(DeckOfTiles.Count - 1);
            return temp;
        }

        //prints the deck of tiles onto console, used only for testing purposes
        //to see if deck is shuffled and working properly
        public void PrintDeck()
        {
            for(int i=0; i<100; i++)
            {
                Debug.WriteLine("Tile Letter : " + DeckOfTiles[i]);
            }
        }
        //sets up the first seven tiles for the computers hand
        public void SetupComputerHand()
        {
            for(int i=0; i<7; i++)
            {
                ComputerHand.Add(DrawTile());
            }
        }
        
        //Function takes two one list of Pairs.
        //Searches left and up for all buildable words from each added tile
        //Then checks each built word to see if it is valid
        // .Value is y coordinate .Key is x coordinate
        //returns total points scored by all the letters otherwise it returns 0 which means not all letters were used
        public static int CheckWords(List<KeyValuePair<int, int>> CoordinatePairs)
        {
            //these are two of the strings returned at the end of the function
            string VerticalWord = "";
            string HorizontalWord = "";

            //keeps track if there is a connection to an island
            int Connect = 0;
            //this is the total score of all valid words returned at the end
            int TotalScore = 0;
            //keeps track to see if all the letters placed on the board are used
            int flag = 0;
            //stores all used words to prevent duplicate words
            List<string> DuplicateWords = new List<string>();

            //stores the X, Y coordinates of used words
            List<KeyValuePair<int, int>> FoundWords = new List<KeyValuePair<int, int>>();

            bool[,] UsedLetters = new bool[15, 15];
            for(int a=0; a<15; a++)
            {
                for(int b=0; b<15; b++)
                {
                    UsedLetters[a,b] = false;
                }
            }

            for (int i = 0; i < CoordinatePairs.Count; i++) //Checks each column for full word
            {       
                //stores x, y coordinates of current word being built
                List<KeyValuePair<int, int>> Storage = new List<KeyValuePair<int, int>>();

                int j = CoordinatePairs[i].Key - 1;//move down the starting row

                string FrontWord = "";
                string tile;
                //Get char at tile position, moving up until empty tile found
                if (j >= 0 && wordsearchBoard[j, CoordinatePairs[i].Value] != " ")
                {
                    Storage.Add(new KeyValuePair<int, int>(j, CoordinatePairs[i].Value));
                }            

                while (j >= 0 && wordsearchBoard[j, CoordinatePairs[i].Value] != " ")
                {             
                    tile = wordsearchBoard[j, CoordinatePairs[i].Value];
                    FrontWord = tile + FrontWord;
                    j--;
                    if(j>=0 && wordsearchBoard[j, CoordinatePairs[i].Value] != " ")
                    {
                        //Get char at tile position, moving up until empty tile found
                        Storage.Add(new KeyValuePair<int, int>(j, CoordinatePairs[i].Value));
                    }
                }
                FrontWord.Trim();

                //main letter that is guarnteed to be placed
                string MainLetter = wordsearchBoard[CoordinatePairs[i].Key, CoordinatePairs[i].Value];
                Storage.Add(new KeyValuePair<int, int>(CoordinatePairs[i].Key, CoordinatePairs[i].Value));
         
                j = CoordinatePairs[i].Key + 1; //Move up a row

                string BackWord = "";
                Storage.Add(new KeyValuePair<int, int>(j, CoordinatePairs[i].Value));

                while (j <= 14 && wordsearchBoard[j, CoordinatePairs[i].Value] != " ")
                {             
                    tile = wordsearchBoard[j, CoordinatePairs[i].Value];
                    BackWord += tile;
                    j++;
                    if(j <= 14 && wordsearchBoard[j, CoordinatePairs[i].Value] != " ")
                    {
                        //Get char at tile position, moving up until empty tile found
                        Storage.Add(new KeyValuePair<int, int>(j, CoordinatePairs[i].Value));
                    }       
                }
                BackWord.Trim();

                int FrontLength = FrontWord.Length;
                int BackLength = BackWord.Length;
                
                for (int a=0; a<=FrontLength; a++)
                {
                    for(int b=0; b<=BackLength; b++)
                    {
                        if (SearchWord.ValidWord(FrontWord.Substring(a, FrontLength-a) + MainLetter + BackWord.Substring(0, BackLength-b)) && !DuplicateWords.Contains(FrontWord.Substring(a, FrontLength - a) + MainLetter + BackWord.Substring(0, BackLength - b)))
                        {
                            List<KeyValuePair<int, int>> Temp = Storage.GetRange(a, FrontLength - a + 1 + BackLength - b);

                            DuplicateWords.Add(FrontWord.Substring(a, FrontLength - a) + MainLetter + BackWord.Substring(0, BackLength - b));
                            //storing all x, y coordinates of valid word to be used to check if all letters used 
                            foreach (KeyValuePair<int, int> item in Temp)
                            {                   
                                FoundWords.Add(new KeyValuePair<int, int>(item.Key, item.Value));
                            }
                            //check to make sure board is not empty and checks if there is any islands
                            if (MainWindow.ValidPairs.Count == 0 || IsConnect(Temp))
                            {
                                Connect = 1;
                            }
           
                            VerticalWord = FrontWord.Substring(a, FrontLength - a) + MainLetter + BackWord.Substring(0, BackLength - b);
                            int points = CalculateScore(VerticalWord);
                            TotalScore += points;
                            MainWindow.OutPutTextBox.AppendText("The word " + VerticalWord + " is worth " + points + " points" + "\n");
                            MainWindow.OutPutTextBox.ScrollToCaret();
                            //marking all letters in the hash table as true
                            foreach (KeyValuePair<int, int> item in FoundWords)
                            {

                                UsedLetters[item.Key, item.Value] = true;
                            }  
                        }
                    }
                }
            }
            //horizontal words do not collide with vertical words for duplicates
            DuplicateWords.Clear();
            for (int i = 0; i < CoordinatePairs.Count; i++) //identical to above loop, but instead checks words to the LEFT
            {

                List<KeyValuePair<int, int>> Storage = new List<KeyValuePair<int, int>>();
  
                int j = CoordinatePairs[i].Value - 1; //move to the left one.

                string FrontWord = "";
                string tile;
                if(j >= 0 && wordsearchBoard[CoordinatePairs[i].Key, j] != " ")
                {
                    Storage.Add(new KeyValuePair<int, int>(CoordinatePairs[i].Key, j));
                }

                while (j >= 0 && wordsearchBoard[CoordinatePairs[i].Key, j] != " ")
                {
                    tile = wordsearchBoard[CoordinatePairs[i].Key, j];
                    FrontWord = tile + FrontWord;
                    j--;
                    if(j >= 0 && wordsearchBoard[CoordinatePairs[i].Key, j] != " ")
                    {
                        //adds tiles to the LEFT of the played tile
                        Storage.Add(new KeyValuePair<int, int>(CoordinatePairs[i].Key, j));
                    }
                   
                }
                FrontWord = FrontWord.Trim();

                //adding original coordinate
                Storage.Add(new KeyValuePair<int, int>(CoordinatePairs[i].Key, CoordinatePairs[i].Value));
                string MainLetter = wordsearchBoard[CoordinatePairs[i].Key, CoordinatePairs[i].Value];

                j = CoordinatePairs[i].Value + 1;
                

                string BackWord = "";
                if (j <= 14 && wordsearchBoard[CoordinatePairs[i].Key, j] != " ")
                {
                    Storage.Add(new KeyValuePair<int, int>(CoordinatePairs[i].Key, j));
                }
               
                while (j <= 14 && wordsearchBoard[CoordinatePairs[i].Key, j] != " ")
                {
                     
                    tile = wordsearchBoard[CoordinatePairs[i].Key, j];
                    BackWord += tile;
                    j++;
                    if(j <= 14 && wordsearchBoard[CoordinatePairs[i].Key, j] != " ")
                    {
                        //adds tiles to the RIGHT of the played tile
                        Storage.Add(new KeyValuePair<int, int>(CoordinatePairs[i].Key, j));
                    }
                    
                }
                //removes trailing white space
                BackWord = BackWord.Trim();
              
                int FrontLength = FrontWord.Length;
                int BackLength = BackWord.Length;
              
                for(int a=0; a<= FrontLength; a++)
                {
                    for(int b=0; b<= BackLength; b++)
                    {
                     
                        if (SearchWord.ValidWord(FrontWord.Substring(a, FrontLength - a) + MainLetter + BackWord.Substring(0, BackLength - b)) && !DuplicateWords.Contains(FrontWord.Substring(a, FrontLength - a) + MainLetter + BackWord.Substring(0, BackLength - b)) )
                        {
                            List<KeyValuePair<int, int>> Temp = Storage.GetRange(a, FrontLength - a + 1 + BackLength - b);
                           
                            DuplicateWords.Add(FrontWord.Substring(a, FrontLength - a) + MainLetter + BackWord.Substring(0, BackLength - b));
                            foreach (KeyValuePair<int, int> item in Temp)
                            {  
                                FoundWords.Add(new KeyValuePair<int, int>(item.Key, item.Value));
                            }
                            //check to make sure board is not empty and checks if there is any islands
                            if (MainWindow.ValidPairs.Count == 0 || IsConnect(Temp))
                            {
                                Connect = 1;
                            }
                            
                            HorizontalWord = FrontWord.Substring(a, FrontLength - a) + MainLetter + BackWord.Substring(0, BackLength - b);
                            int points = CalculateScore(HorizontalWord);
                            TotalScore += points;
                            MainWindow.OutPutTextBox.AppendText("The word " + HorizontalWord + " is worth " + points + " points" + "\n");
                            MainWindow.OutPutTextBox.ScrollToCaret();

                            //marking all letters in the hash table as true
                            foreach (KeyValuePair<int, int> item in FoundWords)
                            {

                                UsedLetters[item.Key, item.Value] = true;
                            }
                           
                            //checking to see if all letters placed on the board are used
                            foreach (KeyValuePair<int, int> item in CoordinatePairs)
                            {
                                if (UsedLetters[item.Key, item.Value] == false)
                                {
                                    flag = 1;
                                }
                            }
                        }
                    }
                }
            }
            if (flag == 0 && Connect == 1)
            {
                return TotalScore;
            }
            //otherwise they are islands
            return -1;
            
        }

        //calculates the points for each word scored and returns the score
        public static int CalculateScore(string word)
        {
            int Total = 0;
            for(int i=0; i<word.Length; i++)
            {
                Total += ScoreTable[word[i].ToString()];
            }
            return Total;
        }
        
        //switches to the computers turn bruteforce with just one tile
        public static void SwitchTurnsEasy()
        {
            for(int i=0; i< ROWS; i++)
            {
                for(int j=0; j< COLS; j++)
                {
                    if(wordsearchBoard[i, j] == " " && CheckAdjacent(i, j))
                    {
                        ComputerCoordinatePairs.Add(new KeyValuePair<int, int>(i, j));
                        int Total;
                        int Counter = 0;
                        foreach (string item in ComputerHand)
                        {
                            wordsearchBoard[i, j] = item;
                            //for easy level just check right away to see if there is a possible word
                            //with just this one tile
                            if ((Total = CheckWords(ComputerCoordinatePairs)) > 0)
                            {
                                MainWindow.OutPutTextBox.AppendText("The computer scored a total of " + Total + " points.\n");
                                MainWindow.OutPutTextBox.ScrollToCaret();
                                MainWindow.ComputerScore.Text = (Convert.ToInt32(MainWindow.ComputerScore.Text) + Convert.ToInt32(Total)).ToString();
                                //once a word is found stop searching that tile for words
                                //and switch to player turn
                                MainWindow.OutPutTextBox.ScrollToCaret();
                                //update frontEndBoard
                                MainWindow.FrontEndBoard[i, j].Text = item;
                                MainWindow.FrontEndBoard[i, j].Enabled = false;
                                //Important for checking future island connections
                                MainWindow.ValidPairs.Add(new KeyValuePair<int, int>(ComputerCoordinatePairs[0].Key, ComputerCoordinatePairs[0].Value));

                                ComputerHand[Counter] = DrawTile();
                                ComputerCoordinatePairs.Clear();
                                return;
                            }
                            Counter++;
                        }
                        wordsearchBoard[i, j] = " ";
                        ComputerCoordinatePairs.Clear();
                    }
                }
            }
            MainWindow.OutPutTextBox.AppendText("The computer passes their turn.\n");
            MainWindow.OutPutTextBox.ScrollToCaret();
        }

        //hard level of computer which uses a smart algorithm with multipe tile placement
        public static void SwitchTurnsHard()
        {
          
        }
        //checks adjacent square to see if anything is placed adjacent on backend board
        //used by computer to bruteforce
        public static bool CheckAdjacent(int x, int y)
        {
            if(x-1 >= 0 && wordsearchBoard[x-1, y] != " ")
            {
                return true;
            }
            else if (x+1 < ROWS && wordsearchBoard[x+1, y] != " ")
            {
                return true;
            }
            else if (y -1 >= 0 && wordsearchBoard[x , y-1] != " ")
            {
                return true;
            }
            else if (y + 1 < COLS && wordsearchBoard[x, y + 1] != " ")
            {
                return true;
            }
            return false;
        }

        //if deck of tiles is empty its game over and the person with most points wins
        public static void GameOver()
        {
            if(Convert.ToInt32(MainWindow.PlayerScore.Text) > Convert.ToInt32(MainWindow.ComputerScore.Text))
            {
                MainWindow.OutPutTextBox.AppendText("You won wordsearch!\n");
            }
            else if(Convert.ToInt32(MainWindow.PlayerScore.Text) < Convert.ToInt32(MainWindow.ComputerScore.Text))
            {
                MainWindow.OutPutTextBox.AppendText("You lost. Sorry.\n");
            }
            else
            {
                MainWindow.OutPutTextBox.AppendText("The game is a tie.\n");
            }
        }
        //checks if any part of the submitted word is connected to of the ValidPairs
        //returns true if there is a connection is found to the main island
        public static bool IsConnect(List<KeyValuePair<int, int>> Storage)
        {
            foreach(KeyValuePair<int, int> item in Storage)
            {
                foreach(KeyValuePair<int, int> Valid in MainWindow.ValidPairs)
                {
                    //if a connection is found to the main island then return true
                    if(item.Key==Valid.Key && item.Value == Valid.Value)
                    {
                        return true;
                    } 
                }
            }
            return false;
        }
    }
}