using CommonDX;
using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace FollowerCatcher
{
    /// <summary>
    /// Represents the game scene.
    /// </summary>
    public class Scene : Component
    {
        private const string WhiteTexturePath = "Content/Textures/white.png.dxt";
        private const string ObjectsTexturePath = "Content/Textures/objects.png.dxt";

        private const string TreeBenchLightmapPath = "Content/Textures/ArbolBanco.png.dxt";
        private const string TreeBenchModelPath = "Content/Models/ArbolBanco.dae.mdl";
        private const string TreeParkingLightmapPath = "Content/Textures/ArbolParking.png.dxt";
        private const string TreeParkingModelPath = "Content/Models/ArbolParking.dae.mdl";
        private const string WindowMailboxLightmapPath = "Content/Textures/VentanaBuzon.png.dxt";
        private const string WindowMailboxModelPath = "Content/Models/VentanaBuzon.dae.mdl";
        private const string WindowHidrantLightmapPath = "Content/Textures/VentanaRiego.png.dxt";
        private const string WindowHidrantModelPath = "Content/Models/VentanaRiego.dae.mdl";

        private const string RoadModelPath = "Content/Models/Carretera.dae.mdl";
        private const string RoadTexturePath = "Content/Textures/Carretera.png.dxt";
        private const string WallModelPath = "Content/Models/Pared1.dae.mdl";
        private const string WallTexturePath = "Content/Textures/Pared1.png.dxt";
        private const string WallRModelPath = "Content/Models/Pared1_d.dae.mdl";

        private const string IconeModelPath = "Content/Models/icon.dae.mdl";
        private const string IconPictureModelPath = "Content/Models/iconPicture.dae.mdl";
        private const string SkateModelPath = "Content/Models/Monopatin.dae.mdl";
        private const string SkateTexturePath = "Content/Textures/Monopatin.png.dxt";
        private const string HipsterModelPath = "Content/Models/Hypster.dae.mdl";
        private const string HipsterTexturePath = "Content/Textures/hypsterTexture.png.dxt";

        private const string SmallCarModelPath = "Content/Models/Car1.dae.mdl";
        private const string BigCarModelPath = "Content/Models/Car2.dae.mdl";
        private const string CarsTexturePath = "Content/Textures/cars.png.dxt";

        private const float PathWidth = 20.0f;

        private Stopwatch clock;
        private InputLayout layout;
        private VertexShader vertexShader;
        private PixelShader pixelShader;
        private SamplerState sampler;
        private List<Entity> parts;
        private float totalTime;
        private Texture white;
        private List<MapObject> mapObjects;
        private TwitterClient client;
        private Model icon;
        private Model iconPicture;
        private List<Entity> collidableObjects;
        private Renderer renderer;
        private List<Entity> tempChilds;
        private EntityFactory factory;
        internal Entity playerEntity;
        internal RoadPaths playerPosition;
        private List<Entity> collidedIcons;
        private List<Entity> drawQueue;
        private BlendState blend;
        private RasterizerState raster;
        private Vector3 cameraPosition;
        private float accumulator;
        private const float updateFrequency = 1.0f / 60.0f;
        private float currentTime;
        private float nextCarAdd;
        private float nextIconAdd;
        private Random randomizer;
        private float roadDistance;
        private DeviceManager deviceManager;
        private bool gameStarted;
        private bool gamePaused;
        private float nextTweetSearch;
        public event EventHandler GameEnded;

        /// <summary>
        /// Initializes a new instance of the <see cref="Scene"/> class.
        /// </summary>
        public Scene()
        {
            totalTime = 0;

            mapObjects = new List<MapObject>();
            collidedIcons = new List<Entity>();
            drawQueue = new List<Entity>();

            cameraPosition = new Vector3(0, 70, -70);

            nextCarAdd = 0;
            nextIconAdd = 0;
            nextTweetSearch = 30.0f;
            randomizer = new Random();

            gameStarted = false;
            gamePaused = false;
        }

        /// <summary>
        /// Initializes the scene.
        /// </summary>
        /// <param name="devices">DirectX device manager.</param>
        public void Initialize(DeviceManager devices)
        {
            deviceManager = devices;

            SafeDispose(ref layout);
            SafeDispose(ref vertexShader);
            SafeDispose(ref pixelShader);
            SafeDispose(ref sampler);

            var d3dDevice = devices.DeviceDirect3D;
            var d3dContext = devices.ContextDirect3D;

            renderer = new Renderer(d3dDevice, d3dContext);
            factory = new EntityFactory(d3dDevice);

            client = new TwitterClient(d3dDevice, devices.WICFactory);
            client.SearchTweets("#" + MainViewModel.Instance.Hashtag);

            tempChilds = new List<Entity>();
            parts = new List<Entity>();

            collidableObjects = new List<Entity>();

            clock = new Stopwatch();
            clock.Start();
            currentTime = (float)clock.ElapsedMilliseconds / 1000.0f;

            MainViewModel.Instance.Miles = 0;
            MainViewModel.Instance.Followers = 0;
        }

        /// <summary>
        /// Loads the content.
        /// </summary>
        public void LoadContent()
        {
            parts.Clear();
            var d3dDevice = deviceManager.DeviceDirect3D;

            white = Texture.LoadFromFile(d3dDevice, WhiteTexturePath).Result;

            Model treeBench = Model.Load(d3dDevice, TreeBenchModelPath);
            Model treeParking = Model.Load(d3dDevice, TreeParkingModelPath);
            Model windowPost = Model.Load(d3dDevice, WindowMailboxModelPath);
            Model windowWater = Model.Load(d3dDevice, WindowHidrantModelPath);

            Texture treeBenchLm = Texture.LoadFromFile(d3dDevice, TreeBenchLightmapPath).Result;
            Texture treeParkingLm = Texture.LoadFromFile(d3dDevice, TreeParkingLightmapPath).Result;
            Texture windowPostLm = Texture.LoadFromFile(d3dDevice, WindowMailboxLightmapPath).Result;
            Texture windowWaterLm = Texture.LoadFromFile(d3dDevice, WindowHidrantLightmapPath).Result;

            mapObjects.Add(new MapObject(treeBench, treeBenchLm));
            mapObjects.Add(new MapObject(treeParking, treeParkingLm));
            mapObjects.Add(new MapObject(windowPost, windowPostLm));
            mapObjects.Add(new MapObject(windowWater, windowWaterLm));

            Model roadModel = Model.Load(d3dDevice, RoadModelPath);

            var path = Windows.ApplicationModel.Package.Current.InstalledLocation.Path;

            icon = Model.Load(d3dDevice, IconeModelPath);
            iconPicture = Model.Load(d3dDevice, IconPictureModelPath);

            var vertexShaderByteCode = NativeFile.ReadAllBytes(path + "\\Content\\Shaders\\DefaultShaderVS.fxo");
            vertexShader = new VertexShader(d3dDevice, vertexShaderByteCode);
            pixelShader = new PixelShader(d3dDevice, NativeFile.ReadAllBytes(path + "\\Content\\Shaders\\DefaultShaderPS.fxo"));

            layout = new InputLayout(d3dDevice, vertexShaderByteCode, new[]
                    {
                        new InputElement("POSITION", 0, Format.R32G32B32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, Format.R32G32_Float, 12, 0)
                    });

            sampler = new SamplerState(d3dDevice, new SamplerStateDescription()
            {
                Filter = Filter.MinMagMipLinear,
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = Colors.Black,
                ComparisonFunction = Comparison.Never,
                MaximumAnisotropy = 0,
                MipLodBias = 0,
                MinimumLod = -float.MaxValue,
                MaximumLod = float.MaxValue
            });

            float roadDistance = (roadModel.BBox.Maximum.Z - roadModel.BBox.Minimum.Z);

            for (int i = 0; i < 18; i++)
            {
                Entity leftPart = factory.CreateEntity(WallModelPath, WallTexturePath);
                leftPart.Transform = Matrix.RotationX((float)Math.PI) * Matrix.Translation(0, 0, roadDistance * i);
                Entity rightPart = factory.CreateEntity(WallRModelPath, WallTexturePath);
                rightPart.Transform = Matrix.RotationX((float)Math.PI) * Matrix.Translation(0, 0, roadDistance * i);
                Entity roadPart = factory.CreateEntity(RoadModelPath, RoadTexturePath);
                roadPart.Transform = Matrix.Translation(0, 0, roadDistance * i);

                parts.Add(roadPart);
                parts.Add(leftPart);
                parts.Add(rightPart);

                if (randomizer.Next(0, 10) < 7)
                {
                    int item = randomizer.Next(0, mapObjects.Count);
                    Entity child = factory.CreateEntity(mapObjects[item].Model.modelPath, ObjectsTexturePath);
                    child.Transform = Matrix.Identity;
                    leftPart.SetChild(child);
                    leftPart.Lightmap = mapObjects[item].ParentLightmap;
                }
                if (randomizer.Next(0, 10) < 7)
                {
                    int item = randomizer.Next(0, mapObjects.Count);
                    Entity child = factory.CreateEntity(mapObjects[item].Model.modelPath, ObjectsTexturePath);
                    child.Transform = Matrix.Scaling(-1.0f, 1.0f, 1.0f);
                    rightPart.SetChild(child);
                    rightPart.Lightmap = mapObjects[item].ParentLightmap;
                }
            }

            playerEntity = factory.CreateEntity(HipsterModelPath, HipsterTexturePath);
            Entity skate = factory.CreateEntity(SkateModelPath, SkateTexturePath);
            playerEntity.SetChild(skate);

            BlendStateDescription blendDescription = new BlendStateDescription();
            blendDescription.RenderTarget[0].IsBlendEnabled = true;
            blendDescription.RenderTarget[0].BlendOperation = blendDescription.RenderTarget[0].AlphaBlendOperation = SharpDX.Direct3D11.BlendOperation.Add;
            blendDescription.RenderTarget[0].SourceBlend = blendDescription.RenderTarget[0].SourceAlphaBlend = SharpDX.Direct3D11.BlendOption.One;
            blendDescription.RenderTarget[0].DestinationBlend = blendDescription.RenderTarget[0].DestinationAlphaBlend = SharpDX.Direct3D11.BlendOption.InverseSourceAlpha;
            blendDescription.RenderTarget[0].RenderTargetWriteMask = SharpDX.Direct3D11.ColorWriteMaskFlags.All;

            blend = new SharpDX.Direct3D11.BlendState(d3dDevice, blendDescription);

            RasterizerStateDescription rasterDescription = new RasterizerStateDescription();
            rasterDescription.CullMode = CullMode.None;
            rasterDescription.DepthBias = 0;
            rasterDescription.DepthBiasClamp = 0;
            rasterDescription.FillMode = FillMode.Solid;
            rasterDescription.IsAntialiasedLineEnabled = false;
            rasterDescription.IsDepthClipEnabled = true;
            rasterDescription.IsFrontCounterClockwise = false;
            rasterDescription.IsMultisampleEnabled = false;
            rasterDescription.IsScissorEnabled = false;
            rasterDescription.SlopeScaledDepthBias = 0;

            raster = new RasterizerState(d3dDevice, rasterDescription);
        }

        /// <summary>
        /// Renders the scene.
        /// </summary>
        /// <param name="render">Render target to draw contents to.</param>
        public void Render(TargetBase render)
        {
            float newTime = (float)clock.ElapsedMilliseconds / 1000.0f;
            float frameTime = newTime - currentTime;
            currentTime = newTime;

            if (!gameStarted)
            {
                return;
            }

            if (gameStarted && !gamePaused)
            {
                accumulator += frameTime;
            }

            var d3dContext = render.DeviceManager.ContextDirect3D;

            float width = (float)render.RenderTargetSize.Width;
            float height = (float)render.RenderTargetSize.Height;

            renderer.View = Matrix.LookAtLH(cameraPosition, new Vector3(0, 0, 70), Vector3.UnitY);
            renderer.Projection = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, width / (float)height, 0.1f, 1000.0f);

            d3dContext.OutputMerger.BlendState = blend;
            d3dContext.Rasterizer.State = raster;
            d3dContext.OutputMerger.SetTargets(render.DepthStencilView, render.RenderTargetView);
            d3dContext.ClearDepthStencilView(render.DepthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
            d3dContext.ClearRenderTargetView(render.RenderTargetView, Colors.Black);

            d3dContext.InputAssembler.InputLayout = layout;
            d3dContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            d3dContext.VertexShader.SetConstantBuffer(0, renderer.constantBuffer);
            d3dContext.VertexShader.Set(vertexShader);
            d3dContext.PixelShader.SetSampler(0, sampler);
            d3dContext.PixelShader.Set(pixelShader);

            while (accumulator >= updateFrequency)
            {
                drawQueue.Clear();

                parts.Sort(new EntityTextureComparer());

                for (int i = 0; i < parts.Count; i++)
                {
                    Entity currentPart = parts[i];

                    currentPart.Transform = currentPart.Transform * Matrix.Translation(0, 0, -200.0f * updateFrequency);
                    roadDistance = (currentPart.Model.BBox.Maximum.Z - currentPart.Model.BBox.Minimum.Z);

                    // TODO: don't hardcode values
                    if (currentPart.Transform.M43 < -roadDistance)
                    {
                        currentPart.Transform = currentPart.Transform * Matrix.Translation(0, 0, roadDistance * 18);
                    }

                    if (currentPart.Child != null)
                    {
                        tempChilds.Add(currentPart.Child);
                    }

                    if (currentPart.Lightmap != null)
                    {
                        renderer.SetActiveTexture(currentPart.Lightmap, 1);
                    }
                    else
                    {
                        renderer.SetActiveTexture(white, 1);
                    }

                    drawQueue.Add(currentPart);
                }

                tempChilds.Sort(new EntityMeshComparer());

                for (int i = 0; i < tempChilds.Count; i++)
                {
                    Entity currentPart = tempChilds[i];

                    drawQueue.Add(currentPart);
                }

                tempChilds.Clear();

                if (playerEntity != null)
                {
                    playerEntity.Update(updateFrequency);
                    drawQueue.Add(playerEntity);
                    drawQueue.Add(playerEntity.Child);
                    cameraPosition.X = playerEntity.Transform.M41;
                }

                for (int i = 0; i < collidableObjects.Count; i++)
                {
                    Entity currentObject = collidableObjects[i];

                    currentObject.Transform = Matrix.Translation(0, 0, -200.0f * updateFrequency) * currentObject.Transform;
                    // HACK
                    currentObject.RestoreMatrix = true;
                    currentObject.oldMatrix = currentObject.Transform;

                    // Pre-rotation collision
                    if (currentObject.IsCollidable && currentObject.BBox.Intersects(ref playerEntity.BBox))
                    {
                        if (currentObject.IsPickable)
                        {
                            collidedIcons.Add(currentObject);
                            MainViewModel.Instance.Followers++;

                            if (client.Tweets.ContainsKey(currentObject.Texture.TexturePath))
                            {
                                TweetInfo info = client.Tweets[currentObject.Texture.TexturePath];
                                MainViewModel.Instance.ActualTweet = info.TweetText;
                                MainViewModel.Instance.ActualUser = info.Username;
                            }
                        }
                        else
                        {
                            // TODO kill
                            gamePaused = true;

                            if (GameEnded != null)
                            {
                                GameEnded(null, EventArgs.Empty);
                            }
                        }
                    }

                    if (currentObject.Rotates)
                    {
                        currentObject.oldMatrix = Matrix.RotationY(clock.ElapsedMilliseconds * 0.005f + currentObject.EffectDelay) * Matrix.Translation(0, (float)Math.Sin(totalTime / 100.0f + currentObject.EffectDelay) * 5.0f, 0) * currentObject.Transform;
                    }

                    if (client.AvatarTextures.Count > 0 && currentObject.Texture.Equals(white))
                    {
                        currentObject.Texture = client.AvatarTextures[i % client.AvatarTextures.Count];
                    }
                    drawQueue.Add(currentObject);
                }

                for (int i = 0; i < collidedIcons.Count; i++)
                {
                    collidableObjects.Remove(collidedIcons[i]);
                }
                collidedIcons.Clear();
            
                accumulator -= updateFrequency;
                nextCarAdd -= updateFrequency;
                nextIconAdd -= updateFrequency;
                nextTweetSearch -= updateFrequency;
                MainViewModel.Instance.Miles += Math.Round(updateFrequency, 2);

                if (nextTweetSearch <= 0)
                {
                    client.SearchTweets("#" + MainViewModel.Instance.Hashtag);
                    nextTweetSearch = 30.0f;
                }

                if (nextCarAdd <= 0)
                {
                    int numCars = randomizer.Next(2) + 3;

                    for (int i = 0; i < numCars; i++)
                    {
                        int carPlace = randomizer.Next(0, 10);
                        int carType = randomizer.Next(0, 10);
                        bool isBigCar = carType > 6;

                        Entity car = factory.CreateEntity(isBigCar ? BigCarModelPath : SmallCarModelPath, CarsTexturePath);
                        car.Transform = Matrix.Translation(carPlace > 4 ? -car.Model.Bounds.X / 2 : car.Model.Bounds.X / 2, isBigCar ? 6.0f : 0.0f, i * roadDistance * 4 + roadDistance * 16);
                         car.IsCollidable = true;
                        car.IsPickable = false;

                        collidableObjects.Add(car);
                    }

                    nextCarAdd = (float)randomizer.NextDouble() * 2.0f + 5.0f;
                }

                if (nextIconAdd <= 0)
                {
                    int numIcons = randomizer.Next(2, 5) * 5;
                    int iconPlace = randomizer.Next(0, 3) - 1;

                    for (int i = 0; i < numIcons; i++)
                    {
                        Entity icon = factory.CreateEntity(IconPictureModelPath, WhiteTexturePath);
                        icon.Transform = Matrix.Translation(iconPlace * PathWidth, 0, i * PathWidth * 2 + roadDistance * 16);
                        icon.IsCollidable = true;
                        icon.IsPickable = true;
                        icon.Rotates = true;
                        icon.EffectDelay = i * 0.2f;

                        if (client.AvatarTextures.Count > 0)
                        {
                            icon.Texture = client.AvatarTextures[i % client.AvatarTextures.Count];
                        }

                        collidableObjects.Add(icon);
                    }

                    nextIconAdd = (float)randomizer.NextDouble() * 2.0f + 3.0f;
                }
            }

            for (int i = 0; i < drawQueue.Count; i++)
            {
                if (drawQueue[i].RestoreMatrix)
                {
                    Matrix tmp = drawQueue[i].Transform;
                    drawQueue[i].Transform = drawQueue[i].oldMatrix;
                    renderer.DrawEntity(drawQueue[i]);
                    drawQueue[i].Transform = tmp;
                }
                else
                {
                    renderer.DrawEntity(drawQueue[i]);
                }
            }

            totalTime = clock.ElapsedMilliseconds;
        }

        /// <summary>
        /// Moves the player.
        /// </summary>
        /// <param name="direction">The direction.</param>
        public void MovePlayer(RoadPaths direction)
        {
            if (!gameStarted)
            {
                return;
            }

            playerPosition += (int)direction;

            if (playerPosition < RoadPaths.Left)
            {
                playerPosition = RoadPaths.Left;
            }

            if (playerPosition > RoadPaths.Right)
            {
                playerPosition = RoadPaths.Right;
            }

            playerEntity.MoveTo = new Vector3((int)playerPosition * PathWidth, 0, 0);
        }

        /// <summary>
        /// Starts the game.
        /// </summary>
        public void StartGame()
        {
            gameStarted = true;
            gamePaused = false;

            MainViewModel.Instance.Miles = 0;
            MainViewModel.Instance.Followers = 0;

            collidableObjects.Clear();
            tempChilds.Clear();
            collidedIcons.Clear();
            drawQueue.Clear();
        }
    }
}
