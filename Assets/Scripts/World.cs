using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Runtime.InteropServices;
using System;

//[ExecuteInEditMode]
public class World : MonoBehaviour {
	//note :-  make this the first script to update (at least before anything else that would use this plugin),  
		//http://docs.unity3d.com/Documentation/Components/class-ScriptExecution.html
	
	public static World Singleton;
	int Reference =0;
	
	//IntPtr Buffer = IntPtr.Zero; 
	const int BufferSize = 128;
	
	/*
	public int toIF<T1,T2>( T1 t1, T2 t2 ) where T1 : struct where T2 : struct {
		int bs = 0;
		long ba = Buffer.ToInt64();
		if( Marshal.SizeOf( t1 ) + Marshal.SizeOf( t2 ) < BufferSize  ) {
			Marshal.StructureToPtr( t1, new IntPtr( ba+bs), false ); bs += Marshal.SizeOf( t1 );
			Marshal.StructureToPtr( t2, new IntPtr( ba+bs), false ); bs += Marshal.SizeOf( t2 );
			return bs;
		}
		Debug.LogError( "toIF 2 would be buffer over flow " );
		return 0; 
	}*/
	/*
	public struct ToIF {
		public ToIF( IntPtr buff ) {
			BA = buff.ToInt64(); BS = 0;	
		}
		long BA, BS;
		
		public ToIF ad<T>( T t ) where T : struct {  //no operater overloading... todo alternative ?
			if( Marshal.SizeOf( t ) + BS < BufferSize  ) {
				Marshal.StructureToPtr( t, new IntPtr( BA+BS), false ); BS += Marshal.SizeOf( t );
				return this;
			}
			Debug.LogError( "toIF 2 would be buffer over flow " );		
			return this;
		}
	    public static implicit operator IntPtr(ToIF a) {  return new IntPtr(a.BS);  }
	};
	public ToIF toIF() { return new ToIF( Buffer ); }
	public ToIF toIF<T1>( T1 t1 ) where T1 : struct { 
		return (new ToIF( Buffer )).ad(t1); 
	}	
	public ToIF toIF<T1,T2>( T1 t1, T2 t2 ) where T1 : struct where T2 : struct { 
		return (new ToIF( Buffer )).ad(t1).ad(t2); 
	}*/
	
#if UNITY_EDITOR
	public const string LibName = "plg_dbg";


    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CB_DrawLine(ref Vector3 a, ref Vector3 b, ref Vector4 c);
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    public delegate void CB_DrawSphere(ref Vector3 a, float b, ref Vector4 c);

    //  [DllImport(LibName)]
    // static extern void Core_drawGizmos([MarshalAs(UnmanagedType.FunctionPtr)] CB_DrawLine dl, [MarshalAs(UnmanagedType.FunctionPtr)] CB_DrawSphere ds);


    public bool Gen = false;
    void drawLine(ref Vector3 a, ref Vector3 b, ref Vector4 c) {
        Gizmos.color = c;
        Gizmos.DrawLine(a, b);
    }
    void drawSphere(ref Vector3 a, float b, ref Vector4 c) {
        Gizmos.color = c;
        Gizmos.DrawWireSphere(a, b);
    }
    void Update() {
        check();
       // Debug.Log("draw ?");
        //    Core_drawGizmos(new CB_DrawLine(this.drawLine), new CB_DrawSphere(this.drawSphere));

        if(Gen) {
            World_foo();
            Gen = false;
        }
    }
#else
	public const string LibName = "space_cpp";
#endif
	
	[DllImport(LibName)] static extern IntPtr accquire( int a ); //sets up dll stuff, also provides ptr to interface buffer
	[DllImport(LibName)] static extern void release(); 
	[DllImport(LibName)] static extern int World_foo(); 
	[DllImport(LibName)] static extern void World_update( float delta ); 
	
	void Awake () {
		if( Singleton!= null ) Debug.LogError( "World:: Singleton violation" ) ;
		
		Singleton = this;
		grab();
		//Buffer = 
            accquire(BufferSize);
	//	Debug.Log("foo "+World_foo() );
	}


	
	public static World grab() {
		Singleton.Reference++;
		return Singleton;
	}	
	public void drop() {
		if( --Reference == 0 ) release();			
		Debug.Log( "World  - onDrop" );
	}
	
	void OnDestroy() {
		Debug.Log( "World  - onDestroy" );
		drop();		
	}


    public static void reimport() {

        Reimport = true;
    }
    static bool Reimport = true;

    void check() { 
        if(Reimport) {
            if(Application.isPlaying) {
                Reimport = false;
               // Debug.LogError("Core:: reimport ..unhandled");
                return;
            }
            release();
            string str = "Assets/plugins/x64/space_cpp.dll";
            if( AssetDatabase.AssetPathToGUID(str) != null )
                AssetDatabase.DeleteAsset(str);
            if(AssetDatabase.CopyAsset("Assets/plugins/x64/in/space_cpp.dll", str))
                Debug.Log("copied success");
            else
                Debug.LogError("Failed to copy plugin");

            accquire(BufferSize);


            Reimport = false;
        }
    }





    public GameObject WormholeFab;

    public Transform WarpingHosts;
}
