using System;
using System.Collections.Generic;
using UnityEngine;

namespace ThanFramework
{
    public class Definitions : MonoBehaviour
    {

        #region General Vars
        public static GameObject grid;
        //public static GameObject player;
        //public static Goop playerScript;
        public static GameObject defaultDropshadow;
        public static GameObject shadows;

        /*
        public static int blockLayerMask;
        public static int ignoreTranslucentLayer;
        public static int ignoreColliderListLayers;
        public static int raycastLayers;
        public static int raycastObstacles;

        public static int layer_inactiveRayCollider;
        public static int layer_aimCollider;
        public static int layer_raycastOnly;

        public const string baseName = "Base";
        public const string elevName = "Elev";
        public const string floorName = "Floor";
        public const string stairsName = "Stairs";
        public const string sceneName = "Scene";
        */
        public static Vector2 absoluteOne = new Vector2(1, 1);

        public const float axisProportion = .75f; //Diagonal directions are fractioned to keep speed consistant

        public const int visibleUnitWidth = 12;
        public static float visiblePixelsPerUnitRatio;

        #endregion

        #region Event Handling

        private void Reset()
        {
            DefinitionStart();
        }

        public static List<Goop> Goops = new List<Goop>();
        public static Camera mainCamera;

        //public static Rigidbody2D modelGoopRb;
        public static float modelGrav;
        public static float modelDrag;

        public static void DefinitionStart()
        {
            grid = GameObject.Find("Grid");
            mainCamera = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<Camera>();

            visiblePixelsPerUnitRatio = mainCamera.pixelWidth / visibleUnitWidth;

            modelGrav = Goops[0].GetComponent<Rigidbody2D>().gravityScale;
            modelDrag = Goops[0].GetComponent<Rigidbody2D>().drag;

            //player = GameObject.FindWithTag("Player");
            //playerScript = player.GetComponent<Goop>();
            //defaultDropshadow = Resources.Load<GameObject>("GameObjects/dropshadow");
            //shadows = GameObject.Find("Shadows");
            //blockLayerMask = (1 << LayerMask.NameToLayer("blockRay") | 1 << LayerMask.NameToLayer("Wall"));

            /*
            //Maybe change this when fixing beam? instead focus on certain layers rather than avoid

            ignoreColliderListLayers = (1 << LayerMask.NameToLayer("DropShadow"));

            /*
            ignoreTranslucentLayer = ~(1 << LayerMask.NameToLayer("Ignore Raycast") | 1 << LayerMask.NameToLayer("Base_0") | 1 << LayerMask.NameToLayer("Base_-1") | 1 << LayerMask.NameToLayer("Base_-2") | 1 << LayerMask.NameToLayer("Base_-3") |
                1 << LayerMask.NameToLayer("Elev_0") | 1 << LayerMask.NameToLayer("Elev_-1") | 1 << LayerMask.NameToLayer("Elev_-2") | 1 << LayerMask.NameToLayer("Elev_-3") | 1 << LayerMask.NameToLayer("Base_1") |
                1 << LayerMask.NameToLayer("Floor_0") | 1 << LayerMask.NameToLayer("Floor_-1") | 1 << LayerMask.NameToLayer("Floor_-2") | 1 << LayerMask.NameToLayer("Floor_-3") | 1 << LayerMask.NameToLayer("Stairs"));
            */
            /*
            ignoreTranslucentLayer = ~(1 << LayerMask.NameToLayer("Inactive Ray Collider") | 1 << LayerMask.NameToLayer("Ignore Raycast") | 1 << LayerMask.NameToLayer("Base") | 1 << LayerMask.NameToLayer("Elev") | 1 << LayerMask.NameToLayer("Floor") | 1 << LayerMask.NameToLayer("Stairs"));

            raycastLayers = (1 << LayerMask.NameToLayer("aimCollider") | 1 << LayerMask.NameToLayer("Raycast Only") );
            raycastObstacles = (1 << LayerMask.NameToLayer("Raycast Only"));

            layer_inactiveRayCollider = LayerMask.NameToLayer("Inactive Ray Collider");
            layer_aimCollider = LayerMask.NameToLayer("aimCollider");
            layer_raycastOnly = LayerMask.NameToLayer("Raycast Only");
            */

        }

        // Update is called once per frame
        public static void DefinitionsUpdate()
        {
            //if (!player) //if we haven't found player for some reason
            //{
            //    player = GameObject.FindWithTag("Player");
            //}
        }

        #endregion

    }
    /*
    #region Damage

    public interface IKillable
    {
        void Kill();
    }

    public interface IDamageable<T>
    {
        void Damage(T damageTaken);
    }

    #endregion
    */

        /*
    #region Health
    [System.Serializable]
    public class HealthSystem : Component
    {
        bool invincible = false;
        public int health;
        public int healthMax;

        public HealthSystem(int healthMax)
        {
            invincible = false;
            this.healthMax = healthMax;
            health = healthMax;
        }

        public HealthSystem(bool invincible)
        {
            this.invincible = invincible;
            healthMax = 1;
            health = healthMax;
        }

        public event EventHandler OnHealthChanged;

        public void Damage(int damageAmount)
        {
            health -= damageAmount;
            if (health < 0) health = 0;
            if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
        }

        public void Heal(int healAmount)
        {
            health += healAmount;
            if (health > healthMax) health = healthMax;
            if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
        }

    }

    #endregion
    */
    /*
    #region Beam

    public class AimState
    {
        //ID RULING - subclasses must follow this rule:
        //If not a grab function - must be negative (enforced)
        //If is a grab function - must be a multiple of 10 (not currently enforced)
        public const int deselect = 0;
        public const int hover = 1;
        public const int grab = 2;

        public const int deselectCollide = -1;
        public const int grabNoAction = 20;
    }


    [Flags] public enum TeleState
    {
        none =  0,      // 000000

        //grab =  1 << 1, // 000001
        //wall =  1 << 2, // 000010

        //obstruct = grab | wall, //000011

        //SETTING
        //When setting bit use  "teleState |= TeleState.FLAG;"
        //When clearing bit use "teleState &= ~Telestate.FLAG;"
        //When reading individual bit make sure to use teleState.HasFlag()
        able =  1 << 1,  // 000001
        hold =  1 << 2,  // 000010

        //CHECKING - for ease of programming / debugging
        //WARNING: When reading these groups of bits DO NOT USE teleState.HasFlag()
        hover = able, //check for able w/o hold
        obstruct = hold, //check for hold w/o able
        control = able | hold, //combined means object can be controlled
    }


    #endregion
    */

        /*
    #region Elevation

    public class elevationModifier
    {
        public const float flat = 0;
        public const float obj = -0.99f;
        public const float scene = -0.01f;
    }

    #endregion
    */

    /*
    #region Tiles

    public enum TilemapType
    {
        none = 0,      // 000000

        raycast = 1 << 1, // 000001
        floor = 1 << 2, // 000010
        stairs = 1 << 3, // 000100
        invisCollider = 1 << 4, // 001000

        All = ~0,           // 111111
    }

    [System.Serializable]
    public class PaletteTileType //Class gives us necessary values for our AutoBrush custom tile brush
    {
        public ID id;
        public bool usesTilemapLayering = false;
        public sceneID sceneOrganizer;
        public string sceneOrganizerName;
        public bool verticalElevationDrag = false;
        public string parentTag = null;
        public bool usesSupportingStructure;
        public float elevationMod = 0;
        public bool obstructingTile = true;
        public bool requiresBelowCellFree = false;

        public PaletteTileType(ID c_id, bool c_usesTilemapLayering, sceneID c_sceneOrganizer, string c_parentTag, bool c_verticalElevationDrag, bool c_usesSupportingStructure, float c_elevationMod, bool c_obstructingTile, bool c_requiresBelowCellFree)
        {
            id = c_id;
            usesTilemapLayering = c_usesTilemapLayering;
            sceneOrganizer = c_sceneOrganizer;
            sceneOrganizerName = sceneOrganizer.ToString();
            parentTag = c_parentTag;
            verticalElevationDrag = c_verticalElevationDrag;
            usesSupportingStructure = c_usesSupportingStructure;
            elevationMod = c_elevationMod;
            obstructingTile = c_obstructingTile;
            requiresBelowCellFree = c_requiresBelowCellFree;
        }

        [System.Serializable]
        public enum ID { Default, Wall, Scene, Stairs, Object };

        [Flags] public enum sceneID
        {
            None =      0,      // 000000

            _Dynamic =  1 << 1, // 000001
            Wall =      1 << 2, // 000010
            Scene =     1 << 3, // 000100
            Objects =   1 << 4, // 001000

            All = ~0,           // 111111
        };

        public static PaletteTileType Default = new PaletteTileType(ID.Default, false, sceneID._Dynamic, null, false, false, elevationModifier.obj, true, false);
        public static PaletteTileType Wall = new PaletteTileType(ID.Wall, true, sceneID.Wall, "WallTileMap", false, true, elevationModifier.flat, true, false);
        public static PaletteTileType Scene = new PaletteTileType(ID.Scene, true, sceneID.Scene, "WallTileMap", false, true, elevationModifier.scene, true, false);
        public static PaletteTileType Stairs = new PaletteTileType(ID.Stairs, true, sceneID.Scene, "WallTileMap", true, false, elevationModifier.scene, false, true);
        public static PaletteTileType Object = new PaletteTileType(ID.Object, false, sceneID.Objects, null, false, false, elevationModifier.obj, true, false);

        public static PaletteTileType GetPaletteTileType (ID new_id)
        {
            switch (new_id)
            {
                case (ID.Default):
                    return Default;

                case (ID.Wall):
                    return Wall;

                case (ID.Scene):
                    return Scene;

                case (ID.Stairs):
                    return Stairs;

                case (ID.Object):
                    return Object;
            }
            return Object;
        }

        public static List<sceneID> all_sceneIDs = new List<sceneID>((sceneID[])Enum.GetValues(typeof(sceneID)));
    }

    #endregion
    */

    #region Directions

    public class DirectionPlane
    {
        public const int hor = 0;
        public const int horizontal = 0;
        public const int vert = 1;
        public const int vertical = 1;
        public const int fourWay = 4;
        public const int eightWay = 8;
    }

    public class Direction
    {
        public const int none = 0;
        public const int up = 1;
        public const int down = -1;
        public const int left = -2;
        public const int right = 2;
    }

    #endregion

    #region Colors

    public class ColorId
    {
        public static Color launch;

        public static Color control;

        public static Color hover;

        public static Color grab;

        public static Color off;

        public static Color on;

        public static Color enemy;

        public static Color flame;

        public class GreyTint
        {
            public static Color greyBase;
            public static Color regular = new Color(.5f, .5f, .5f, 1);
            public static Color red;
        }
    }

    #endregion

    /*
    #region Colliders

    public enum ColliderType { Default, Circle, Box, Capsule };

    public class CollisionLayerType
    {
        public const int none = 0;
        public const int Base = 1;
        public const int Elev = 2;
        public const int Floor = 3;

        public const int maxElevation = -12;
        public const int minElevation = 0;
    }

    public class ColliderValues
    {
        Vector2 size;
        Vector2 offset;
        CapsuleDirection2D direction = CapsuleDirection2D.Horizontal;

        ColliderValues(CircleCollider2D collider)
        {
            size = new Vector2(collider.radius, collider.radius);
            offset = collider.offset;
        }

        ColliderValues(BoxCollider2D collider)
        {
            size = collider.size;
            offset = collider.offset;
        }

        ColliderValues(CapsuleCollider2D collider)
        {
            size = collider.size;
            offset = collider.offset;
            direction = collider.direction;
        }
    }

    #endregion
    */
}