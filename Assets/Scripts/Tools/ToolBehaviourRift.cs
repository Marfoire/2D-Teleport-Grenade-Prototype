using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;

public class ToolBehaviourRift : MonoBehaviour
{
    //layer mask for the stage
    LayerMask stageMask;

    //the rift tile, it doesn't have any collider data
    public Tile riftTile;

    //the default stage tile, it has grid collider data
    public Tile defaultTile;

    //bool to start the rift's coroutine
    public bool initiateCoroutine;

    //bool to delay when the rift toggles tiles because it toggles tiles too fast for the coroutine
    bool delayRiftTimestep;

    //the tilemap for the stage
    Tilemap stageTilemap;

    //list of the positions of all tiles affected by the rift for when they need to be reenabled
    List<Vector3Int> affectedTiles = new List<Vector3Int>();

    //reference to the player script
    public PlayerController playerReference;

    //boolean to be set to true if the rift has started closing
    bool riftClosing;


    void Awake()
    {
        //make sure the layer mask is a layer mask
        stageMask = LayerMask.GetMask("StageLayer");
    }

    private void ToggleTilesInRange()
    {
        //bounds int type to establish the bounding box in which the rift disables tiles
        BoundsInt fillBounds = new BoundsInt();

        //set the min and max positions of the bounding box, the positions are the bottom left and top right corners of the trigger collider that the rift has
        fillBounds.SetMinMax(stageTilemap.layoutGrid.WorldToCell(transform.position - GetComponent<Collider2D>().bounds.extents), stageTilemap.layoutGrid.WorldToCell(transform.position + GetComponent<Collider2D>().bounds.extents));

        //draw rays to visualize the two points that the bounding box has
        /*
        Debug.DrawRay(transform.position - GetComponent<Collider2D>().bounds.extents, Vector2.up, Color.yellow);
        Debug.DrawRay(transform.position + GetComponent<Collider2D>().bounds.extents, Vector2.up, Color.yellow);
        */

        //tile track is the horizontal index of which tile is being scanned
        int tileTrack = 0;

        //tile track cycles is the vertical index of which tile is being scanned
        int tileTrackCycles = 0;

        //the differences of the max x and y of the bounds multiplied together (2 is added to the x because the difference should be inclusive, it ends up being 6x6)
        for (int i = 0; i <= ((fillBounds.max.x - fillBounds.min.x + 2) * (fillBounds.max.y - fillBounds.min.y)); i++)
        {
            //if the x index of the current tile is greater than the x range of the bounding box
            if (tileTrack > fillBounds.max.x - fillBounds.min.x)
            {
                //reset the x index to 0
                tileTrack = 0;

                //a cycle has been completed so add one to the y index
                tileTrackCycles++;
            }

            //draw ray for debugging
            //Debug.DrawRay(stageTilemap.layoutGrid.GetCellCenterWorld(new Vector3Int(fillBounds.min.x + tileTrack, fillBounds.min.y + tileTrackCycles, 0)), Vector2.up, Color.red);

            //raycasthit check to make sure that the tile that is being checked is part of the stage and should be put into the rift
            RaycastHit2D stageTileInRift = Physics2D.Raycast(stageTilemap.layoutGrid.GetCellCenterWorld(new Vector3Int(fillBounds.min.x + tileTrack, fillBounds.min.y + tileTrackCycles, 0)), Vector2.up, 0.1f, stageMask);

            //if the raycast came back with a result and the result is tagged with stage
            if (stageTileInRift && stageTileInRift.collider.tag == "Stage")
            {
                //set the tile at that location to be a rift tile
                stageTilemap.SetTile(new Vector3Int(fillBounds.min.x + tileTrack, fillBounds.min.y + tileTrackCycles, fillBounds.min.z), riftTile);

                //if the position observed is not already part of the affected tile position list
                if (!affectedTiles.Contains(new Vector3Int(fillBounds.min.x + tileTrack, fillBounds.min.y + tileTrackCycles, fillBounds.min.z)))
                {
                    //add the position of this tile to the list so it can be reactivated when the rift closes
                    affectedTiles.Add(new Vector3Int(fillBounds.min.x + tileTrack, fillBounds.min.y + tileTrackCycles, fillBounds.min.z));
                }
            }
            //add one to the x index for the next cycle
            tileTrack++;
        }
        //reset the delay timestep boolean because this cycle is done
        delayRiftTimestep = false;
    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        //if a stage piece is staying in the rift
        if (collision.tag == "Stage")
        {
            //get the mf tile map
            stageTilemap = collision.GetComponent<Tilemap>();
            //delay the rift timestep so the size of the trigger can keep up with the toggling of the tiles
            delayRiftTimestep = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if a stage piece entered the rift
        if (collision.tag == "Stage")
        {
            //get that mf tilemap
            stageTilemap = collision.GetComponent<Tilemap>();
            //delay the rift timestep because it tends to check for tiles to disable too early
            delayRiftTimestep = true;
        }

        //if the player entered the rift
        if (collision.tag == "Player")
        {
            //tell them that they are in the rift
            playerReference.GetComponent<PlayerController>().inRift = true;
        }
    }


    private void OnTriggerExit2D(Collider2D collision)
    {
        //if the player leaves the rift before it closes
        if (collision.tag == "Player" && riftClosing == false)
        {
            //they ain't in the rift
            collision.GetComponent<PlayerController>().inRift = false;
        }
    }

    //expand and shrink controls what the rift does during its lifespan
    public IEnumerator ExpandAndShrink()
    {
        //while the rift is not at full size
        while (transform.localScale.y < 865)
        {
            //increase the rift's x and y scale by 86.5
            transform.localScale = new Vector3(transform.localScale.x + 86.5f, transform.localScale.y + 86.5f, transform.localScale.z);

            //wait very briefly before running again
            yield return new WaitForSecondsRealtime(0.008f);
        }

        
        if(GetComponent<SpriteRenderer>().color.a > 0)
        {
            GetComponent<SpriteRenderer>().color = new Color(GetComponent<SpriteRenderer>().color.r, GetComponent<SpriteRenderer>().color.g, GetComponent<SpriteRenderer>().color.b, GetComponent<SpriteRenderer>().color.a - 0.0008823529f);
            
        }

        yield return new WaitForSecondsRealtime(11);

        riftClosing = true;

        //while the rift is still visible
        while (transform.localScale.y > 0)
        {
            //shrink the rift by 86.5 on x and y scale
            transform.localScale = new Vector3(transform.localScale.x - 86.5f, transform.localScale.y - 86.5f, transform.localScale.z);
            //wait very briefly before continuing
            yield return new WaitForSecondsRealtime(0.008f);
        }

        //set the tiles back to normal
        ReactivateTiles();

    }

    private void ReactivateTiles()
    {

        //for all the stored tile positions which contain rift tiles
        foreach (Vector3Int tilePos in affectedTiles)
        {
            Debug.DrawRay(stageTilemap.layoutGrid.GetCellCenterWorld(tilePos), Vector2.up, Color.red);

            if (playerReference != null) {
                if (playerReference.GetComponent<PlayerController>().inRift == true)
                {
                    RaycastHit2D playerKillCheck = Physics2D.Raycast(stageTilemap.layoutGrid.GetCellCenterWorld(tilePos), Vector2.up, 0.1f);
                    if (playerKillCheck)
                    {
                        if (playerKillCheck.collider.tag == "Player")
                        {
                            //game end the player here
                            playerReference.GetComponent<PlayerHealth>().PlayerDeath();
                        }
                    }


                }
            }

            //set the tiles back to the default stage tile
            stageTilemap.SetTile(tilePos, defaultTile);
        }

        //there is no longer an active rift
        playerReference.activeRift = false;

        //destroy the rift once it shrinks beyond visibility
        Destroy(gameObject);
    }


    private void Update()
    {
        //if it is time to start the coroutine
        if (initiateCoroutine == true)
        {
            //start the coroutine
            StartCoroutine(ExpandAndShrink());
        }

        //if there is a player reference
        if (playerReference != null)
        {
            //establish a listener for when the player invokes the rift override event
            playerReference.overrideRift.AddListener(ReactivateTiles);
        }

    }

    private void LateUpdate()
    {
        //when the rift is ready to disable tiles
        if (delayRiftTimestep)
        {
            //disable the tiles in range
            ToggleTilesInRange();
        }
    }


}
