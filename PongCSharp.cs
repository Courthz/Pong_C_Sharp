using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;

namespace BaseGame
{
    enum GameScreen { LOGO, TITLE, GAMEPLAY, ENDING }

    public class PongCSharp : Game
    {
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        private Texture2D texture;
        private SpriteFont font;

        private KeyboardManager keyboard;
        private KeyboardState keyboardState;

        private MouseManager mouse;
        private MouseState mouseState;

        int screenWidth = 800;
        int screenHeight = 450;

        Random rand;

        int height = 80;

        int playerPosX = 0;
        int playerPosY;   //= (screenHeight/2) - (height/2);
        int playerSpeedY = 3;

        int enemyPosX;   //= screenWidth - 20;
        int enemyPosY;   //= (screenHeight/2) - (height/2);
        int enemySpeedY = 3;

        int ballPosX;   //= screenWidth/2;
        int ballPosY;     //= screenHeight/2;
        int ballRadius = 15;
        int ballSpeedX;                    //rand() % 3 + 3;
        int ballSpeedY;                    //rand() % 3 + 3;
        //if (rand()%2) ballSpeedX *= -1;
        //if (rand()%2) ballSpeedY *= -1;       // La bola tomará una velocidad (x,y) entre 3->5 o -5->-3

        int playerLife = 200;
        int enemyLife = 200;

        bool parpadeo = false;

        Color LogoColor;               //black  LogoColor.A = 0;
        Color TitleColor;           //green	 TitleColor.A = 0;
        Color EnterColor;            //lime	 EnterColor.A = 0;
        Color endingColor;          //green

        bool pause = false;
        bool choque = false;

        int secondsCounter = 99;

        int framesCounter = 0;

        GameScreen screen = GameScreen.LOGO;

        Color raywhite = new Color(245, 245, 245, 255);

        float time;

        Texture2D textureBall;
        Texture2D textureRec;

        Rectangle playerRec;
        Rectangle enemyRec;
        Rectangle playerLifeRec;
        Rectangle enemyLifeRec;

        public PongCSharp()
        {
            // Create the graphics device manager
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            // Setup the relative directory to the executable directory for content loading with ContentManager
            Content.RootDirectory = "Content";

            // Initialize input keyboard system
            keyboard = new KeyboardManager(this);

            // Initialize input mouse system
            mouse = new MouseManager(this);
        }

        protected override void Initialize()
        {
            // Setup window size and apply changes
            graphicsDeviceManager.PreferredBackBufferWidth = screenWidth;
            graphicsDeviceManager.PreferredBackBufferHeight = screenHeight;
            graphicsDeviceManager.ApplyChanges();

            Window.Title = "Pong C#";

            rand = new Random();

            if (rand.Next(2) == 1) ballSpeedY = rand.Next(-5, -2);
            else ballSpeedY = rand.Next(3, 6);

            if (rand.Next(2) == 1) ballSpeedX = rand.Next(-5, -2);
            else ballSpeedX = rand.Next(3, 6);

            playerPosY = (screenHeight / 2) - (height / 2);

            enemyPosX = screenWidth - 20;
            enemyPosY = (screenHeight / 2) - (height / 2);


            ballPosX = screenWidth / 2;
            ballPosY = screenHeight / 2;

            LogoColor = new Color();
            LogoColor = Color.Black;
            LogoColor.A = 0;

            TitleColor = new Color();
            TitleColor = Color.Green;
            TitleColor.A = 0;

            EnterColor = new Color();
            EnterColor = Color.Lime;
            EnterColor.A = 0;

            endingColor = new Color();
            endingColor = Color.Green;

            TargetElapsedTime = new TimeSpan(0, 0, 1 / 60);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            // Instantiate a SpriteBatch
            spriteBatch = new SpriteBatch(GraphicsDevice);

            textureBall = Content.Load<Texture2D>("ball");
            textureRec = Content.Load<Texture2D>("rectangle");

            // Load a system font
            font = Content.Load<SpriteFont>("myfont");

            base.LoadContent();
        }

        protected override void UnloadContent()
        {
            // Unload spriteBatch object
            spriteBatch.Dispose();

            base.UnloadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            // Get the current state of the keyboard
            keyboardState = keyboard.GetState();

            // Get the current state of the mouse
            mouseState = mouse.GetState();

            // Check if Escape key has been pressed to exit
            if (keyboardState.IsKeyPressed(Keys.Escape)) Exit();

            // Count total execution time in seconds
            time = (float)gameTime.TotalGameTime.TotalSeconds;

            switch (screen)
            {
                case GameScreen.LOGO:
                    {
                        // Update LOGO screen data here!

                        // TODO: Logo fadeIn and fadeOut logic...............(0.5p)
                        framesCounter++;

                        if (framesCounter <= 90) LogoColor.A++;
                        if (framesCounter > 90 && framesCounter <= 210) LogoColor.A = LogoColor.A;
                        if (framesCounter > 210 && framesCounter <= 300) LogoColor.A--;
                        if (framesCounter > 300) screen = GameScreen.TITLE;

                    } break;
                case GameScreen.TITLE:
                    {
                        // Update TITLE screen data here!

                        // TODO: Title animation logic.......................(0.5p)

                        if (TitleColor.A != 255) TitleColor.A++;

                        // TODO: "PRESS ENTER" logic.........................(0.5p)
                        framesCounter++;

                        if ((parpadeo == false) && (framesCounter >= 30))
                        {
                            framesCounter = 0;
                            EnterColor.A = 0;
                            parpadeo = !parpadeo;
                        }
                        if ((parpadeo == true) && (framesCounter >= 30))
                        {
                            framesCounter = 0;
                            EnterColor.A = 255;
                            parpadeo = !parpadeo;
                        }
                        if (keyboardState.IsKeyPressed(Keys.Enter)) screen = GameScreen.GAMEPLAY;

                    } break;
                case GameScreen.GAMEPLAY:
                    {
                        // Update GAMEPLAY screen data here!

                        // TODO: Player movement logic.......................(0.2p)

                        if (!pause)
                        {
                            if (keyboardState.IsKeyDown(Keys.Up)) playerPosY -= playerSpeedY;
                            if (keyboardState.IsKeyDown(Keys.Down)) playerPosY += playerSpeedY;
                        }
                        // TODO: Enemy movement logic (IA)...................(1p)

                        if (!pause)
                        {
                            if (ballSpeedY < 0) enemySpeedY = rand.Next(-5, 0);
                            if (ballSpeedY > 0) enemySpeedY = rand.Next(1, 6);

                            enemyPosY += enemySpeedY;
                        }

                        // TODO: Collision detection (ball-player) logic.....(0.5p) [*Cada vez que choca, la bola aumenta su velocidad (quería hacerlo añadiendo ballSpeedY++ y ballSpeedX++,
                        //pero al alcanzar una velocidad muy alta, la bola escapa de los limites de la ventana)]

                        if (((ballPosX - ballRadius) <= 20) && ((ballPosY + ballRadius) >= playerPosY) && ((ballPosY - ballRadius) <= (playerPosY + height))) ballSpeedX = -ballSpeedX;
                        // TODO: Collision detection (ball-enemy) logic......(0.5p)

                        if (((ballPosX + ballRadius) >= (screenWidth - 20)) && ((ballPosY + ballRadius) >= enemyPosY) && ((ballPosY - ballRadius) <= (enemyPosY + height))) ballSpeedX = -ballSpeedX;
                        // TODO: Collision detection (ball-limits) logic.....(1p)

                        if (((ballPosY - ballRadius) <= 0) || ((ballPosY + ballRadius) >= screenHeight)) ballSpeedY = -ballSpeedY;
                        if (((ballPosX - ballRadius) <= 0) || ((ballPosX + ballRadius) >= screenWidth)) ballSpeedX = -ballSpeedX;

                        // TODO: Collision detection (bars-limits) logic.....(??p)

                        if (playerPosY <= 0) playerPosY = 0;
                        if ((playerPosY + height) >= screenHeight) playerPosY = screenHeight - height;

                        if (enemyPosY <= 0) enemyPosY = 0;
                        if ((enemyPosY + height) >= screenHeight) enemyPosY = screenHeight - height;
                        // TODO: Life bars decrease logic....................(1p)

                        if ((ballPosX - ballRadius) <= 0) playerLife -= 20;
                        if ((ballPosX + ballRadius) >= screenWidth) enemyLife -= 20;

                        // TODO: Time counter logic..........................(0.2p)

                        if (!pause)
                        {
                            framesCounter++;
                            if ((framesCounter >= 60) && (secondsCounter != 0))
                            {
                                framesCounter = 0;
                                secondsCounter--;
                            }
                        }
                        // TODO: Game ending logic...........................(0.2p)

                        if ((enemyLife == 0) || (playerLife == 0) || (secondsCounter == 0)) screen = GameScreen.ENDING;

                        // TODO: Pause button logic..........................(0.2p)

                        if (keyboardState.IsKeyPressed(Keys.P)) pause = !pause;

                        // TODO: Ball movement logic.........................(0.2p)

                        if (!pause)
                        {
                            ballPosX += ballSpeedX;
                            ballPosY += ballSpeedY;
                        }

                        //El color del mensaje cambiará según el mensaje que salga, WIN = GREEN, LOSE = RED, DRAW = YELLOW [ ** P.]

                        if ((playerLife == 0) || ((enemyLife > playerLife) && (secondsCounter == 0))) endingColor = Color.Red;
                        if ((playerLife == enemyLife) && (secondsCounter == 0)) endingColor = Color.Yellow;

                    } break;
                case GameScreen.ENDING:
                    {
                        // Update END screen data here!

                        // TODO: Replay / Exit game logic....................(0.5p)

                        if (keyboardState.IsKeyPressed(Keys.R))
                        {
                            secondsCounter = 99;
                            framesCounter = 0;
                            playerLife = 200;
                            enemyLife = 200;
                            ballPosX = screenWidth / 2;
                            ballPosY = screenHeight / 2;
                            playerPosY = (screenHeight / 2) - (height / 2);
                            enemyPosY = (screenHeight / 2) - (height / 2);
                            screen = GameScreen.GAMEPLAY;
                        }
                        //Exit game logic se contiene en la funcion WindowShouldClose.
                        //Para que el mensaje de WIN, LOSE y DRAW vaya desapareciendo a partir de los 4 segundos hasta que el canal alfa del color llegue a 0. [** P.]

                        framesCounter++;
                        if ((framesCounter >= 240) && (endingColor.A > 0)) endingColor.A--;

                    } break;
                default: break;
            }
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(raywhite);

            spriteBatch.Begin();

            switch (screen)
            {
                case GameScreen.LOGO:
                    {
                        // Draw LOGO screen here!

                        // TODO: Draw Logo...............................(0.2p)

                        spriteBatch.DrawString(font, "DANIEL GAMES", new Vector2(screenWidth / 2 - 70, screenHeight / 2 - 50), LogoColor); //"DANIEL GAMES", screenWidth / 2 - 310, screenHeight / 2 - 50, 80, LogoColor

                    } break;
                case GameScreen.TITLE:
                    {
                        // Draw TITLE screen here!

                        // TODO: Draw Title..............................(0.2p)

                        spriteBatch.DrawString(font, "EPILEPTIC BALL", new Vector2(screenWidth / 2 - 80, screenHeight / 2 - 100), TitleColor);

                        // TODO: Draw "PRESS ENTER" message..............(0.2p)

                        spriteBatch.DrawString(font, "PRESS ENTER", new Vector2(screenWidth / 2 - 65, screenHeight / 2 + 100), EnterColor);

                    } break;
                case GameScreen.GAMEPLAY:
                    {
                        // Draw GAMEPLAY screen here!

                        // TODO: Draw player and enemy...................(0.2p)
                        playerRec = new Rectangle(playerPosX, playerPosY, 20, height);
                        enemyRec = new Rectangle(enemyPosX, enemyPosY, 20, height);
                        //DrawRectangle(playerPosX, playerPosY, 20, height, BLACK);
                        spriteBatch.Draw(textureRec, playerRec, Color.Black);
                       // DrawRectangle(enemyPosX, enemyPosY, 20, height, BLACK);
                        spriteBatch.Draw(textureRec, enemyRec, Color.Black);
                        // TODO: Draw Ball...............................(***p)

                        //DrawCircle(ballPosX, ballPosY, ballRadius, BLACK);
                        spriteBatch.Draw(textureBall, new Vector2(ballPosX, ballPosY), null, Color.Black, 0, new Vector2(textureBall.Width/2, textureBall.Height/2), (float)ballRadius/(textureBall.Width/2), SpriteEffects.None, 0);

                        // TODO: Draw player and enemy life bars.........(0.5p)
                        playerLifeRec = new Rectangle(100, 10, playerLife, 20);
                        enemyLifeRec = new Rectangle(500, 10, enemyLife, 20);

                        spriteBatch.Draw(textureRec, new Rectangle(99, 9, 202, 22), Color.Black);
                        //DrawRectangle(99, 9, 202, 22, BLACK);
                        spriteBatch.Draw(textureRec, new Rectangle(100, 10, 200, 20), Color.Red);
                       // DrawRectangle(100, 10, 200, 20, RED);
                        spriteBatch.Draw(textureRec, playerLifeRec, Color.Green);
                        //DrawRectangle(100, 10, playerLife, 20, GREEN);
                        spriteBatch.Draw(textureRec, new Rectangle(499, 9, 202, 22), Color.Black);
                        //DrawRectangle(499, 9, 202, 22, BLACK);
                        spriteBatch.Draw(textureRec, new Rectangle(500, 10, 200, 20), Color.Red);
                       // DrawRectangle(500, 10, 200, 20, RED);
                        spriteBatch.Draw(textureRec, enemyLifeRec, Color.Green);
                        //DrawRectangle(500, 10, enemyLife, 20, GREEN);


                        // TODO: Draw time counter.......................(0.5p)

                       // spriteBatch.DrawString(FormatText("%02i", secondsCounter), screenWidth / 2 - 40, 5, 80, BLACK); //"%02i" para que la variable int tenga 2 digitos (01,02,03...)
                        spriteBatch.DrawString(font, secondsCounter.ToString("00"), new Vector2(10, 10), Color.Black);

                        // TODO: Draw pause message when required........(0.5p)

                        if (pause) spriteBatch.DrawString(font, "PAUSE", new Vector2(screenWidth / 2 - 35, screenHeight / 2 - 20), Color.Black);

                    } break;
                case GameScreen.ENDING:
                    {
                        // Draw END screen here!

                        // TODO: Draw ending message (win or loose)......(0.2p)

                        spriteBatch.DrawString(font, "IF YOU WANT TO PLAY AGAIN, PRESS 'R'", new Vector2(screenWidth / 2 - 240, screenHeight / 2 - 50), Color.Black);
                        spriteBatch.DrawString(font, "IF YOU WANT TO EXIT, PRESS 'ESCAPE'", new Vector2(screenWidth / 2 - 240, screenHeight / 2 + 50), Color.Black);
                        if ((enemyLife == 0) || ((enemyLife < playerLife) && (secondsCounter == 0))) spriteBatch.DrawString(font, "YOU WIN", new Vector2(screenWidth / 2 - 40, screenHeight / 2 - 20), endingColor);
                        if ((playerLife == 0) || ((enemyLife > playerLife) && (secondsCounter == 0))) spriteBatch.DrawString(font, "YOU LOSE", new Vector2(screenWidth / 2 - 40, screenHeight / 2 - 20), endingColor);
                        if ((playerLife == enemyLife) && (secondsCounter == 0)) spriteBatch.DrawString(font, "DRAW", new Vector2(screenWidth / 2 - 30, screenHeight / 2 - 20), endingColor);

                    } break;
                default: break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
