xof 0303txt 0032

// DirectX 9.0 file
// Creator: Ultimate Unwrap3D Pro v3.18
// Time: Sat Oct 10 08:04:19 2009

// Start of Templates

template VertexDuplicationIndices {
 <b8d65549-d7c9-4995-89cf-53a9a8b031e3>
 DWORD nIndices;
 DWORD nOriginalVertices;
 array DWORD indices[nIndices];
}

template FVFData {
 <b6e70a0e-8ef9-4e83-94ad-ecc8b0c04897>
 DWORD dwFVF;
 DWORD nDWords;
 array DWORD data[nDWords];
}

template Header {
 <3D82AB43-62DA-11cf-AB39-0020AF71E433>
 WORD major;
 WORD minor;
 DWORD flags;
}

template Vector {
 <3D82AB5E-62DA-11cf-AB39-0020AF71E433>
 FLOAT x;
 FLOAT y;
 FLOAT z;
}

template Coords2d {
 <F6F23F44-7686-11cf-8F52-0040333594A3>
 FLOAT u;
 FLOAT v;
}

template Matrix4x4 {
 <F6F23F45-7686-11cf-8F52-0040333594A3>
 array FLOAT matrix[16];
}

template ColorRGBA {
 <35FF44E0-6C7C-11cf-8F52-0040333594A3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
 FLOAT alpha;
}

template ColorRGB {
 <D3E16E81-7835-11cf-8F52-0040333594A3>
 FLOAT red;
 FLOAT green;
 FLOAT blue;
}

template IndexedColor {
 <1630B820-7842-11cf-8F52-0040333594A3>
 DWORD index;
 ColorRGBA indexColor;
}

template Material {
 <3D82AB4D-62DA-11cf-AB39-0020AF71E433>
 ColorRGBA faceColor;
 FLOAT power;
 ColorRGB specularColor;
 ColorRGB emissiveColor;
 [...]
}

template TextureFilename {
 <A42790E1-7810-11cf-8F52-0040333594A3>
 STRING filename;
}

template MeshFace {
 <3D82AB5F-62DA-11cf-AB39-0020AF71E433>
 DWORD nFaceVertexIndices;
 array DWORD faceVertexIndices[nFaceVertexIndices];
}

template MeshTextureCoords {
 <F6F23F40-7686-11cf-8F52-0040333594A3>
 DWORD nTextureCoords;
 array Coords2d textureCoords[nTextureCoords];
}

template MeshMaterialList {
 <F6F23F42-7686-11cf-8F52-0040333594A3>
 DWORD nMaterials;
 DWORD nFaceIndexes;
 array DWORD faceIndexes[nFaceIndexes];
 [Material]
}

template MeshNormals {
 <F6F23F43-7686-11cf-8F52-0040333594A3>
 DWORD nNormals;
 array Vector normals[nNormals];
 DWORD nFaceNormals;
 array MeshFace faceNormals[nFaceNormals];
}

template MeshVertexColors {
 <1630B821-7842-11cf-8F52-0040333594A3>
 DWORD nVertexColors;
 array IndexedColor vertexColors[nVertexColors];
}

template Mesh {
 <3D82AB44-62DA-11cf-AB39-0020AF71E433>
 DWORD nVertices;
 array Vector vertices[nVertices];
 DWORD nFaces;
 array MeshFace faces[nFaces];
 [...]
}

template FrameTransformMatrix {
 <F6F23F41-7686-11cf-8F52-0040333594A3>
 Matrix4x4 frameMatrix;
}

template Frame {
 <3D82AB46-62DA-11cf-AB39-0020AF71E433>
 [...]
}

template FloatKeys {
 <10DD46A9-775B-11cf-8F52-0040333594A3>
 DWORD nValues;
 array FLOAT values[nValues];
}

template TimedFloatKeys {
 <F406B180-7B3B-11cf-8F52-0040333594A3>
 DWORD time;
 FloatKeys tfkeys;
}

template AnimationKey {
 <10DD46A8-775B-11cf-8F52-0040333594A3>
 DWORD keyType;
 DWORD nKeys;
 array TimedFloatKeys keys[nKeys];
}

template AnimationOptions {
 <E2BF56C0-840F-11cf-8F52-0040333594A3>
 DWORD openclosed;
 DWORD positionquality;
}

template Animation {
 <3D82AB4F-62DA-11cf-AB39-0020AF71E433>
 [...]
}

template AnimationSet {
 <3D82AB50-62DA-11cf-AB39-0020AF71E433>
 [Animation]
}

template XSkinMeshHeader {
 <3CF169CE-FF7C-44ab-93C0-F78F62D172E2>
 WORD nMaxSkinWeightsPerVertex;
 WORD nMaxSkinWeightsPerFace;
 WORD nBones;
}

template SkinWeights {
 <6F0D123B-BAD2-4167-A0D0-80224F25FABB>
 STRING transformNodeName;
 DWORD nWeights;
 array DWORD vertexIndices[nWeights];
 array FLOAT weights[nWeights];
 Matrix4x4 matrixOffset;
}

template AnimTicksPerSecond {
 <9E415A43-7BA6-4a73-8743-B73D47E88476>
 DWORD AnimTicksPerSecond;
}

AnimTicksPerSecond {
 4800;
}

// Start of Frames

Frame Body {
   FrameTransformMatrix {
    1.000000, 0.000000, 0.000000, 0.000000,
    0.000000, 1.000000, 0.000000, 0.000000,
    0.000000, 0.000000, 1.000000, 0.000000,
    0.000000, 0.000000, 0.000000, 1.000000;;
   }

   Mesh staticMesh {
    156;
    -22.697853; 15.848879; -37.586735;,
    -16.933067; 14.702650; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -12.045374; 11.435778; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -8.778502; 6.548085; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -7.632273; 0.783298; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -8.778502; -4.982689; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -12.045374; -9.870382; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -16.933067; -13.137255; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -22.699055; -14.283484; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -28.463840; -13.137255; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -33.351532; -9.870382; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -36.618404; -4.982689; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -37.764633; 0.783298; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -36.618404; 6.548085; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -33.351532; 11.435778; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -28.463840; 14.702650; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -22.697853; 15.848879; -37.586735;,
    -22.697853; 0.783298; -40.583271;,
    -22.697853; 28.622002; -29.052498;,
    -12.045374; 26.502560; -29.052498;,
    -3.013716; 20.467436; -29.052498;,
    3.021408; 11.435778; -29.052498;,
    5.140850; 0.783298; -29.052498;,
    3.021408; -9.870382; -29.052498;,
    -3.013716; -18.902040; -29.052498;,
    -12.045374; -24.937164; -29.052498;,
    -22.699055; -27.056606; -29.052498;,
    -33.351532; -24.937164; -29.052498;,
    -42.383194; -18.902040; -29.052498;,
    -48.418316; -9.870382; -29.052498;,
    -50.537758; 0.783298; -29.052498;,
    -48.418316; 11.435778; -29.052498;,
    -42.383194; 20.467436; -29.052498;,
    -33.351532; 26.502560; -29.052498;,
    -33.351532; 26.502560; -29.052498;,
    -22.697853; 28.622002; -29.052498;,
    -22.697853; 37.156239; -16.279375;,
    -8.778502; 34.386787; -16.279375;,
    3.021408; 26.502560; -16.279375;,
    10.905636; 14.702650; -16.279375;,
    13.675088; 0.782097; -16.279375;,
    10.905636; -13.137255; -16.279375;,
    3.021408; -24.937164; -16.279375;,
    -8.778502; -32.821392; -16.279375;,
    -22.699055; -35.590843; -16.279375;,
    -36.618404; -32.821392; -16.279375;,
    -48.418316; -24.937164; -16.279375;,
    -56.302544; -13.137255; -16.279375;,
    -59.071995; 0.783298; -16.279375;,
    -56.302544; 14.702650; -16.279375;,
    -48.418316; 26.502560; -16.279375;,
    -36.618404; 34.386787; -16.279375;,
    -36.618404; 34.386787; -16.279375;,
    -22.697853; 37.156239; -16.279375;,
    -22.697853; 40.152775; -1.213795;,
    -7.632273; 37.156239; -1.213795;,
    5.140850; 28.622002; -1.213795;,
    13.675088; 15.848879; -1.213795;,
    16.671623; 0.782097; -1.213795;,
    13.675088; -14.283484; -1.213795;,
    5.140850; -27.056606; -1.213795;,
    -7.632273; -35.590843; -1.213795;,
    -22.699055; -38.587379; -1.213795;,
    -37.764633; -35.590843; -1.213795;,
    -50.537758; -27.056606; -1.213795;,
    -59.071995; -14.283484; -1.213795;,
    -62.068531; 0.783298; -1.213795;,
    -59.071995; 15.848879; -1.213795;,
    -50.537758; 28.622002; -1.213795;,
    -37.764633; 37.156239; -1.213795;,
    -37.764633; 37.156239; -1.213795;,
    -22.697853; 40.152775; -1.213795;,
    -22.697853; 37.156239; 13.852987;,
    -8.778502; 34.386787; 13.852987;,
    3.021408; 26.502560; 13.852987;,
    10.905636; 14.702650; 13.852987;,
    13.675088; 0.782097; 13.852987;,
    10.905636; -13.137255; 13.852987;,
    3.021408; -24.937164; 13.852987;,
    -8.778502; -32.821392; 13.852987;,
    -22.699055; -35.590843; 13.852987;,
    -36.618404; -32.821392; 13.852987;,
    -48.418316; -24.937164; 13.852987;,
    -56.302544; -13.137255; 13.852987;,
    -59.071995; 0.783298; 13.852987;,
    -56.302544; 14.702650; 13.852987;,
    -48.418316; 26.502560; 13.852987;,
    -36.618404; 34.386787; 13.852987;,
    -36.618404; 34.386787; 13.852987;,
    -22.697853; 37.156239; 13.852987;,
    -22.697853; 28.622002; 26.626110;,
    -12.045374; 26.502560; 26.626110;,
    -3.013716; 20.467436; 26.626110;,
    3.021408; 11.435778; 26.626110;,
    5.140850; 0.783298; 26.626110;,
    3.021408; -9.870382; 26.626110;,
    -3.013716; -18.902040; 26.626110;,
    -12.045374; -24.937164; 26.626110;,
    -22.699055; -27.056606; 26.626110;,
    -33.351532; -24.937164; 26.626110;,
    -42.383194; -18.902040; 26.626110;,
    -48.418316; -9.870382; 26.626110;,
    -50.537758; 0.783298; 26.626110;,
    -48.418316; 11.435778; 26.626110;,
    -42.383194; 20.467436; 26.626110;,
    -33.351532; 26.502560; 26.626110;,
    -33.351532; 26.502560; 26.626110;,
    -22.697853; 28.622002; 26.626110;,
    -22.697853; 15.848879; 35.160347;,
    -16.933067; 14.702650; 35.160347;,
    -12.045374; 11.435778; 35.160347;,
    -8.778502; 6.548085; 35.160347;,
    -7.632273; 0.783298; 35.160347;,
    -8.778502; -4.982689; 35.160347;,
    -12.045374; -9.870382; 35.160347;,
    -16.933067; -13.137255; 35.160347;,
    -22.699055; -14.283484; 35.160347;,
    -28.463840; -13.137255; 35.160347;,
    -33.351532; -9.870382; 35.160347;,
    -36.618404; -4.982689; 35.160347;,
    -37.764633; 0.783298; 35.160347;,
    -36.618404; 6.548085; 35.160347;,
    -33.351532; 11.435778; 35.160347;,
    -28.463840; 14.702650; 35.160347;,
    -22.697853; 15.848879; 35.160347;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;,
    -22.697853; 0.783298; 38.156883;;
    224;
    3;0, 1, 2;,
    3;1, 3, 4;,
    3;3, 5, 6;,
    3;5, 7, 8;,
    3;7, 9, 10;,
    3;9, 11, 12;,
    3;11, 13, 14;,
    3;13, 15, 16;,
    3;15, 17, 18;,
    3;17, 19, 20;,
    3;19, 21, 22;,
    3;21, 23, 24;,
    3;23, 25, 26;,
    3;25, 27, 28;,
    3;27, 29, 30;,
    3;29, 31, 32;,
    3;33, 1, 0;,
    3;33, 34, 1;,
    3;34, 3, 1;,
    3;34, 35, 3;,
    3;35, 5, 3;,
    3;35, 36, 5;,
    3;36, 7, 5;,
    3;36, 37, 7;,
    3;37, 9, 7;,
    3;37, 38, 9;,
    3;38, 11, 9;,
    3;38, 39, 11;,
    3;39, 13, 11;,
    3;39, 40, 13;,
    3;40, 15, 13;,
    3;40, 41, 15;,
    3;41, 17, 15;,
    3;41, 42, 17;,
    3;42, 19, 17;,
    3;42, 43, 19;,
    3;43, 21, 19;,
    3;43, 44, 21;,
    3;44, 23, 21;,
    3;44, 45, 23;,
    3;45, 25, 23;,
    3;45, 46, 25;,
    3;46, 27, 25;,
    3;46, 47, 27;,
    3;47, 29, 27;,
    3;47, 48, 29;,
    3;49, 31, 29;,
    3;49, 50, 31;,
    3;51, 34, 33;,
    3;51, 52, 34;,
    3;52, 35, 34;,
    3;52, 53, 35;,
    3;53, 36, 35;,
    3;53, 54, 36;,
    3;54, 37, 36;,
    3;54, 55, 37;,
    3;55, 38, 37;,
    3;55, 56, 38;,
    3;56, 39, 38;,
    3;56, 57, 39;,
    3;57, 40, 39;,
    3;57, 58, 40;,
    3;58, 41, 40;,
    3;58, 59, 41;,
    3;59, 42, 41;,
    3;59, 60, 42;,
    3;60, 43, 42;,
    3;60, 61, 43;,
    3;61, 44, 43;,
    3;61, 62, 44;,
    3;62, 45, 44;,
    3;62, 63, 45;,
    3;63, 46, 45;,
    3;63, 64, 46;,
    3;64, 47, 46;,
    3;64, 65, 47;,
    3;65, 48, 47;,
    3;65, 66, 48;,
    3;67, 50, 49;,
    3;67, 68, 50;,
    3;69, 52, 51;,
    3;69, 70, 52;,
    3;70, 53, 52;,
    3;70, 71, 53;,
    3;71, 54, 53;,
    3;71, 72, 54;,
    3;72, 55, 54;,
    3;72, 73, 55;,
    3;73, 56, 55;,
    3;73, 74, 56;,
    3;74, 57, 56;,
    3;74, 75, 57;,
    3;75, 58, 57;,
    3;75, 76, 58;,
    3;76, 59, 58;,
    3;76, 77, 59;,
    3;77, 60, 59;,
    3;77, 78, 60;,
    3;78, 61, 60;,
    3;78, 79, 61;,
    3;79, 62, 61;,
    3;79, 80, 62;,
    3;80, 63, 62;,
    3;80, 81, 63;,
    3;81, 64, 63;,
    3;81, 82, 64;,
    3;82, 65, 64;,
    3;82, 83, 65;,
    3;83, 66, 65;,
    3;83, 84, 66;,
    3;85, 68, 67;,
    3;85, 86, 68;,
    3;87, 70, 69;,
    3;87, 88, 70;,
    3;88, 71, 70;,
    3;88, 89, 71;,
    3;89, 72, 71;,
    3;89, 90, 72;,
    3;90, 73, 72;,
    3;90, 91, 73;,
    3;91, 74, 73;,
    3;91, 92, 74;,
    3;92, 75, 74;,
    3;92, 93, 75;,
    3;93, 76, 75;,
    3;93, 94, 76;,
    3;94, 77, 76;,
    3;94, 95, 77;,
    3;95, 78, 77;,
    3;95, 96, 78;,
    3;96, 79, 78;,
    3;96, 97, 79;,
    3;97, 80, 79;,
    3;97, 98, 80;,
    3;98, 81, 80;,
    3;98, 99, 81;,
    3;99, 82, 81;,
    3;99, 100, 82;,
    3;100, 83, 82;,
    3;100, 101, 83;,
    3;101, 84, 83;,
    3;101, 102, 84;,
    3;103, 86, 85;,
    3;103, 104, 86;,
    3;105, 88, 87;,
    3;105, 106, 88;,
    3;106, 89, 88;,
    3;106, 107, 89;,
    3;107, 90, 89;,
    3;107, 108, 90;,
    3;108, 91, 90;,
    3;108, 109, 91;,
    3;109, 92, 91;,
    3;109, 110, 92;,
    3;110, 93, 92;,
    3;110, 111, 93;,
    3;111, 94, 93;,
    3;111, 112, 94;,
    3;112, 95, 94;,
    3;112, 113, 95;,
    3;113, 96, 95;,
    3;113, 114, 96;,
    3;114, 97, 96;,
    3;114, 115, 97;,
    3;115, 98, 97;,
    3;115, 116, 98;,
    3;116, 99, 98;,
    3;116, 117, 99;,
    3;117, 100, 99;,
    3;117, 118, 100;,
    3;118, 101, 100;,
    3;118, 119, 101;,
    3;119, 102, 101;,
    3;119, 120, 102;,
    3;121, 104, 103;,
    3;121, 122, 104;,
    3;123, 106, 105;,
    3;123, 124, 106;,
    3;124, 107, 106;,
    3;124, 125, 107;,
    3;125, 108, 107;,
    3;125, 126, 108;,
    3;126, 109, 108;,
    3;126, 127, 109;,
    3;127, 110, 109;,
    3;127, 128, 110;,
    3;128, 111, 110;,
    3;128, 129, 111;,
    3;129, 112, 111;,
    3;129, 130, 112;,
    3;130, 113, 112;,
    3;130, 131, 113;,
    3;131, 114, 113;,
    3;131, 132, 114;,
    3;132, 115, 114;,
    3;132, 133, 115;,
    3;133, 116, 115;,
    3;133, 134, 116;,
    3;134, 117, 116;,
    3;134, 135, 117;,
    3;135, 118, 117;,
    3;135, 136, 118;,
    3;136, 119, 118;,
    3;136, 137, 119;,
    3;137, 120, 119;,
    3;137, 138, 120;,
    3;138, 122, 121;,
    3;138, 139, 122;,
    3;140, 124, 123;,
    3;141, 125, 124;,
    3;142, 126, 125;,
    3;143, 127, 126;,
    3;144, 128, 127;,
    3;145, 129, 128;,
    3;146, 130, 129;,
    3;147, 131, 130;,
    3;148, 132, 131;,
    3;149, 133, 132;,
    3;150, 134, 133;,
    3;151, 135, 134;,
    3;152, 136, 135;,
    3;153, 137, 136;,
    3;154, 138, 137;,
    3;155, 139, 138;;

   MeshNormals {
    156;
    0.075771; 0.381079; -0.921432;,
    0.139427; 0.395261; -0.907925;,
    0.038759; 0.194932; -0.980051;,
    0.280071; 0.311837; -0.907920;,
    0.110446; 0.165242; -0.980050;,
    0.378070; 0.180930; -0.907925;,
    0.165242; 0.110446; -0.980050;,
    0.418530; 0.022477; -0.907925;,
    0.194932; 0.038759; -0.980051;,
    0.395261; -0.139417; -0.907927;,
    0.194932; -0.038751; -0.980051;,
    0.311834; -0.280068; -0.907922;,
    0.165235; -0.110441; -0.980051;,
    0.180923; -0.378068; -0.907927;,
    0.110438; -0.165231; -0.980052;,
    0.022462; -0.418526; -0.907927;,
    0.038748; -0.194920; -0.980054;,
    -0.139424; -0.395255; -0.907929;,
    -0.038755; -0.194914; -0.980054;,
    -0.280065; -0.311831; -0.907923;,
    -0.110434; -0.165224; -0.980054;,
    -0.378066; -0.180922; -0.907928;,
    -0.165224; -0.110434; -0.980054;,
    -0.418526; -0.022462; -0.907927;,
    -0.194917; -0.038748; -0.980054;,
    -0.395256; 0.139425; -0.907928;,
    -0.194917; 0.038756; -0.980054;,
    -0.311834; 0.280068; -0.907922;,
    -0.165231; 0.110438; -0.980052;,
    -0.180924; 0.378070; -0.907926;,
    -0.110441; 0.165235; -0.980051;,
    -0.087663; 0.440842; -0.893294;,
    -0.038751; 0.194932; -0.980051;,
    0.129841; 0.652712; -0.746397;,
    0.261971; 0.656269; -0.707588;,
    0.493126; 0.506091; -0.707601;,
    0.649270; 0.278851; -0.707595;,
    0.706558; 0.009174; -0.707596;,
    0.656269; -0.261966; -0.707591;,
    0.506091; -0.493126; -0.707601;,
    0.278843; -0.649273; -0.707595;,
    0.009159; -0.706558; -0.707596;,
    -0.261971; -0.656269; -0.707588;,
    -0.493126; -0.506091; -0.707601;,
    -0.649273; -0.278843; -0.707595;,
    -0.706558; -0.009159; -0.707596;,
    -0.656269; 0.261971; -0.707589;,
    -0.506091; 0.493126; -0.707601;,
    -0.421245; 0.630421; -0.652013;,
    -0.129825; 0.652722; -0.746391;,
    -0.147942; 0.743641; -0.652006;,
    0.175570; 0.882422; -0.436470;,
    0.348969; 0.855226; -0.383157;,
    0.649667; 0.656591; -0.383173;,
    0.851487; 0.357996; -0.383157;,
    0.923675; 0.004874; -0.383146;,
    0.855226; -0.348969; -0.383157;,
    0.656591; -0.649667; -0.383173;,
    0.357993; -0.851487; -0.383159;,
    0.004871; -0.923675; -0.383146;,
    -0.348969; -0.855226; -0.383157;,
    -0.649667; -0.656591; -0.383173;,
    -0.851487; -0.357993; -0.383159;,
    -0.923675; -0.004871; -0.383146;,
    -0.855226; 0.348969; -0.383157;,
    -0.656591; 0.649667; -0.383173;,
    -0.527159; 0.788979; -0.315620;,
    -0.175552; 0.882417; -0.436488;,
    -0.185130; 0.930655; -0.315606;,
    0.194687; 0.978715; -0.064912;,
    0.382677; 0.923882; -0.000016;,
    0.707107; 0.707107; -0.000008;,
    0.923885; 0.382670; 0.000000;,
    1.000000; -0.000008; -0.000008;,
    0.923882; -0.382677; -0.000016;,
    0.707107; -0.707107; -0.000008;,
    0.382670; -0.923885; 0.000000;,
    -0.000008; -1.000000; -0.000008;,
    -0.382677; -0.923882; -0.000016;,
    -0.707107; -0.707107; -0.000008;,
    -0.923885; -0.382670; 0.000000;,
    -1.000000; 0.000008; -0.000008;,
    -0.923882; 0.382677; -0.000016;,
    -0.707107; 0.707107; -0.000008;,
    -0.554381; 0.829729; 0.064896;,
    -0.194672; 0.978719; -0.064895;,
    -0.194672; 0.978719; 0.064897;,
    0.185146; 0.930656; 0.315596;,
    0.358002; 0.851487; 0.383151;,
    0.656593; 0.649669; 0.383166;,
    0.855231; 0.348963; 0.383150;,
    0.923678; -0.004883; 0.383139;,
    0.851487; -0.357999; 0.383153;,
    0.649669; -0.656593; 0.383166;,
    0.348963; -0.855231; 0.383150;,
    -0.004887; -0.923678; 0.383139;,
    -0.358002; -0.851487; 0.383151;,
    -0.656593; -0.649669; 0.383166;,
    -0.855231; -0.348963; 0.383150;,
    -0.923678; 0.004887; 0.383139;,
    -0.851487; 0.358002; 0.383151;,
    -0.649669; 0.656593; 0.383166;,
    -0.499850; 0.748079; 0.436494;,
    -0.185131; 0.930659; 0.315594;,
    -0.175554; 0.882427; 0.436466;,
    0.147956; 0.743638; 0.652005;,
    0.278851; 0.649270; 0.707595;,
    0.506091; 0.493126; 0.707601;,
    0.656272; 0.261969; 0.707587;,
    0.706558; -0.009161; 0.707596;,
    0.649273; -0.278843; 0.707595;,
    0.493126; -0.506091; 0.707601;,
    0.261964; -0.656272; 0.707589;,
    -0.009175; -0.706558; 0.707596;,
    -0.278851; -0.649270; 0.707595;,
    -0.506091; -0.493126; 0.707601;,
    -0.656272; -0.261964; 0.707589;,
    -0.706558; 0.009175; 0.707596;,
    -0.649270; 0.278851; 0.707595;,
    -0.493126; 0.506091; 0.707601;,
    -0.369770; 0.553326; 0.746391;,
    -0.147943; 0.743647; 0.651998;,
    -0.129823; 0.652715; 0.746398;,
    0.087678; 0.440839; 0.893294;,
    0.180930; 0.378070; 0.907925;,
    0.311837; 0.280071; 0.907920;,
    0.395261; 0.139427; 0.907925;,
    0.418532; -0.022462; 0.907924;,
    0.378070; -0.180924; 0.907926;,
    0.280068; -0.311834; 0.907922;,
    0.139416; -0.395258; 0.907928;,
    -0.022476; -0.418525; 0.907927;,
    -0.180928; -0.378063; 0.907928;,
    -0.311831; -0.280065; 0.907923;,
    -0.395256; -0.139415; 0.907929;,
    -0.418525; 0.022477; 0.907927;,
    -0.378065; 0.180929; 0.907927;,
    -0.280068; 0.311834; 0.907922;,
    -0.139417; 0.395261; 0.907927;,
    -0.075755; 0.381079; 0.921434;,
    0.038759; 0.194932; 0.980051;,
    0.110446; 0.165242; 0.980050;,
    0.165242; 0.110446; 0.980050;,
    0.194932; 0.038759; 0.980051;,
    0.194932; -0.038751; 0.980051;,
    0.165235; -0.110441; 0.980051;,
    0.110438; -0.165231; 0.980052;,
    0.038748; -0.194920; 0.980054;,
    -0.038755; -0.194914; 0.980054;,
    -0.110434; -0.165224; 0.980054;,
    -0.165224; -0.110434; 0.980054;,
    -0.194917; -0.038748; 0.980054;,
    -0.194917; 0.038756; 0.980054;,
    -0.165231; 0.110438; 0.980052;,
    -0.110441; 0.165235; 0.980051;,
    -0.038751; 0.194932; 0.980051;;
    224;
    3;0, 1, 2;,
    3;1, 3, 4;,
    3;3, 5, 6;,
    3;5, 7, 8;,
    3;7, 9, 10;,
    3;9, 11, 12;,
    3;11, 13, 14;,
    3;13, 15, 16;,
    3;15, 17, 18;,
    3;17, 19, 20;,
    3;19, 21, 22;,
    3;21, 23, 24;,
    3;23, 25, 26;,
    3;25, 27, 28;,
    3;27, 29, 30;,
    3;29, 31, 32;,
    3;33, 1, 0;,
    3;33, 34, 1;,
    3;34, 3, 1;,
    3;34, 35, 3;,
    3;35, 5, 3;,
    3;35, 36, 5;,
    3;36, 7, 5;,
    3;36, 37, 7;,
    3;37, 9, 7;,
    3;37, 38, 9;,
    3;38, 11, 9;,
    3;38, 39, 11;,
    3;39, 13, 11;,
    3;39, 40, 13;,
    3;40, 15, 13;,
    3;40, 41, 15;,
    3;41, 17, 15;,
    3;41, 42, 17;,
    3;42, 19, 17;,
    3;42, 43, 19;,
    3;43, 21, 19;,
    3;43, 44, 21;,
    3;44, 23, 21;,
    3;44, 45, 23;,
    3;45, 25, 23;,
    3;45, 46, 25;,
    3;46, 27, 25;,
    3;46, 47, 27;,
    3;47, 29, 27;,
    3;47, 48, 29;,
    3;49, 31, 29;,
    3;49, 50, 31;,
    3;51, 34, 33;,
    3;51, 52, 34;,
    3;52, 35, 34;,
    3;52, 53, 35;,
    3;53, 36, 35;,
    3;53, 54, 36;,
    3;54, 37, 36;,
    3;54, 55, 37;,
    3;55, 38, 37;,
    3;55, 56, 38;,
    3;56, 39, 38;,
    3;56, 57, 39;,
    3;57, 40, 39;,
    3;57, 58, 40;,
    3;58, 41, 40;,
    3;58, 59, 41;,
    3;59, 42, 41;,
    3;59, 60, 42;,
    3;60, 43, 42;,
    3;60, 61, 43;,
    3;61, 44, 43;,
    3;61, 62, 44;,
    3;62, 45, 44;,
    3;62, 63, 45;,
    3;63, 46, 45;,
    3;63, 64, 46;,
    3;64, 47, 46;,
    3;64, 65, 47;,
    3;65, 48, 47;,
    3;65, 66, 48;,
    3;67, 50, 49;,
    3;67, 68, 50;,
    3;69, 52, 51;,
    3;69, 70, 52;,
    3;70, 53, 52;,
    3;70, 71, 53;,
    3;71, 54, 53;,
    3;71, 72, 54;,
    3;72, 55, 54;,
    3;72, 73, 55;,
    3;73, 56, 55;,
    3;73, 74, 56;,
    3;74, 57, 56;,
    3;74, 75, 57;,
    3;75, 58, 57;,
    3;75, 76, 58;,
    3;76, 59, 58;,
    3;76, 77, 59;,
    3;77, 60, 59;,
    3;77, 78, 60;,
    3;78, 61, 60;,
    3;78, 79, 61;,
    3;79, 62, 61;,
    3;79, 80, 62;,
    3;80, 63, 62;,
    3;80, 81, 63;,
    3;81, 64, 63;,
    3;81, 82, 64;,
    3;82, 65, 64;,
    3;82, 83, 65;,
    3;83, 66, 65;,
    3;83, 84, 66;,
    3;85, 68, 67;,
    3;85, 86, 68;,
    3;87, 70, 69;,
    3;87, 88, 70;,
    3;88, 71, 70;,
    3;88, 89, 71;,
    3;89, 72, 71;,
    3;89, 90, 72;,
    3;90, 73, 72;,
    3;90, 91, 73;,
    3;91, 74, 73;,
    3;91, 92, 74;,
    3;92, 75, 74;,
    3;92, 93, 75;,
    3;93, 76, 75;,
    3;93, 94, 76;,
    3;94, 77, 76;,
    3;94, 95, 77;,
    3;95, 78, 77;,
    3;95, 96, 78;,
    3;96, 79, 78;,
    3;96, 97, 79;,
    3;97, 80, 79;,
    3;97, 98, 80;,
    3;98, 81, 80;,
    3;98, 99, 81;,
    3;99, 82, 81;,
    3;99, 100, 82;,
    3;100, 83, 82;,
    3;100, 101, 83;,
    3;101, 84, 83;,
    3;101, 102, 84;,
    3;103, 86, 85;,
    3;103, 104, 86;,
    3;105, 88, 87;,
    3;105, 106, 88;,
    3;106, 89, 88;,
    3;106, 107, 89;,
    3;107, 90, 89;,
    3;107, 108, 90;,
    3;108, 91, 90;,
    3;108, 109, 91;,
    3;109, 92, 91;,
    3;109, 110, 92;,
    3;110, 93, 92;,
    3;110, 111, 93;,
    3;111, 94, 93;,
    3;111, 112, 94;,
    3;112, 95, 94;,
    3;112, 113, 95;,
    3;113, 96, 95;,
    3;113, 114, 96;,
    3;114, 97, 96;,
    3;114, 115, 97;,
    3;115, 98, 97;,
    3;115, 116, 98;,
    3;116, 99, 98;,
    3;116, 117, 99;,
    3;117, 100, 99;,
    3;117, 118, 100;,
    3;118, 101, 100;,
    3;118, 119, 101;,
    3;119, 102, 101;,
    3;119, 120, 102;,
    3;121, 104, 103;,
    3;121, 122, 104;,
    3;123, 106, 105;,
    3;123, 124, 106;,
    3;124, 107, 106;,
    3;124, 125, 107;,
    3;125, 108, 107;,
    3;125, 126, 108;,
    3;126, 109, 108;,
    3;126, 127, 109;,
    3;127, 110, 109;,
    3;127, 128, 110;,
    3;128, 111, 110;,
    3;128, 129, 111;,
    3;129, 112, 111;,
    3;129, 130, 112;,
    3;130, 113, 112;,
    3;130, 131, 113;,
    3;131, 114, 113;,
    3;131, 132, 114;,
    3;132, 115, 114;,
    3;132, 133, 115;,
    3;133, 116, 115;,
    3;133, 134, 116;,
    3;134, 117, 116;,
    3;134, 135, 117;,
    3;135, 118, 117;,
    3;135, 136, 118;,
    3;136, 119, 118;,
    3;136, 137, 119;,
    3;137, 120, 119;,
    3;137, 138, 120;,
    3;138, 122, 121;,
    3;138, 139, 122;,
    3;140, 124, 123;,
    3;141, 125, 124;,
    3;142, 126, 125;,
    3;143, 127, 126;,
    3;144, 128, 127;,
    3;145, 129, 128;,
    3;146, 130, 129;,
    3;147, 131, 130;,
    3;148, 132, 131;,
    3;149, 133, 132;,
    3;150, 134, 133;,
    3;151, 135, 134;,
    3;152, 136, 135;,
    3;153, 137, 136;,
    3;154, 138, 137;,
    3;155, 139, 138;;
   }

   MeshTextureCoords {
    156;
    0.000977; 0.875000;,
    0.062500; 0.875000;,
    0.031250; 1.000000;,
    0.125000; 0.875000;,
    0.093750; 1.000000;,
    0.187500; 0.875000;,
    0.156250; 1.000000;,
    0.250000; 0.875000;,
    0.218750; 1.000000;,
    0.312500; 0.875000;,
    0.281250; 1.000000;,
    0.375000; 0.875000;,
    0.343750; 1.000000;,
    0.437500; 0.875000;,
    0.406250; 1.000000;,
    0.500000; 0.875000;,
    0.468750; 1.000000;,
    0.562500; 0.875000;,
    0.531250; 1.000000;,
    0.625000; 0.875000;,
    0.593750; 1.000000;,
    0.687500; 0.875000;,
    0.656250; 1.000000;,
    0.750000; 0.875000;,
    0.718750; 1.000000;,
    0.812500; 0.875000;,
    0.781250; 1.000000;,
    0.875000; 0.875000;,
    0.843750; 1.000000;,
    0.937500; 0.875000;,
    0.906250; 1.000000;,
    0.997070; 0.875000;,
    0.967773; 0.998047;,
    0.000977; 0.750000;,
    0.062500; 0.750000;,
    0.125000; 0.750000;,
    0.187500; 0.750000;,
    0.250000; 0.750000;,
    0.312500; 0.750000;,
    0.375000; 0.750000;,
    0.437500; 0.750000;,
    0.500000; 0.750000;,
    0.562500; 0.750000;,
    0.625000; 0.750000;,
    0.687500; 0.750000;,
    0.750000; 0.750000;,
    0.812500; 0.750000;,
    0.875000; 0.750000;,
    0.937500; 0.750000;,
    0.937500; 0.751953;,
    0.997070; 0.751953;,
    0.000977; 0.625000;,
    0.062500; 0.625000;,
    0.125000; 0.625000;,
    0.187500; 0.625000;,
    0.250000; 0.625000;,
    0.312500; 0.625000;,
    0.375000; 0.625000;,
    0.437500; 0.625000;,
    0.500000; 0.625000;,
    0.562500; 0.625000;,
    0.625000; 0.625000;,
    0.687500; 0.625000;,
    0.750000; 0.625000;,
    0.812500; 0.625000;,
    0.875000; 0.625000;,
    0.937500; 0.625000;,
    0.938477; 0.625000;,
    0.997070; 0.625000;,
    0.000977; 0.500000;,
    0.062500; 0.500000;,
    0.125000; 0.500000;,
    0.187500; 0.500000;,
    0.250000; 0.500000;,
    0.312500; 0.500000;,
    0.375000; 0.500000;,
    0.437500; 0.500000;,
    0.500000; 0.500000;,
    0.562500; 0.500000;,
    0.625000; 0.500000;,
    0.687500; 0.500000;,
    0.750000; 0.500000;,
    0.812500; 0.500000;,
    0.875000; 0.500000;,
    0.937500; 0.500000;,
    0.936523; 0.498047;,
    0.997070; 0.498047;,
    0.000977; 0.375000;,
    0.062500; 0.375000;,
    0.125000; 0.375000;,
    0.187500; 0.375000;,
    0.250000; 0.375000;,
    0.312500; 0.375000;,
    0.375000; 0.375000;,
    0.437500; 0.375000;,
    0.500000; 0.375000;,
    0.562500; 0.375000;,
    0.625000; 0.375000;,
    0.687500; 0.375000;,
    0.750000; 0.375000;,
    0.812500; 0.375000;,
    0.875000; 0.375000;,
    0.937500; 0.375000;,
    0.938477; 0.371094;,
    0.997070; 0.371094;,
    0.000977; 0.250000;,
    0.062500; 0.250000;,
    0.125000; 0.250000;,
    0.187500; 0.250000;,
    0.250000; 0.250000;,
    0.312500; 0.250000;,
    0.375000; 0.250000;,
    0.437500; 0.250000;,
    0.500000; 0.250000;,
    0.562500; 0.250000;,
    0.625000; 0.250000;,
    0.687500; 0.250000;,
    0.750000; 0.250000;,
    0.812500; 0.250000;,
    0.875000; 0.250000;,
    0.937500; 0.250000;,
    0.937500; 0.248047;,
    0.997070; 0.248047;,
    0.000977; 0.125000;,
    0.062500; 0.125000;,
    0.125000; 0.125000;,
    0.187500; 0.125000;,
    0.250000; 0.125000;,
    0.312500; 0.125000;,
    0.375000; 0.125000;,
    0.437500; 0.125000;,
    0.500000; 0.125000;,
    0.562500; 0.125000;,
    0.625000; 0.125000;,
    0.687500; 0.125000;,
    0.750000; 0.125000;,
    0.812500; 0.125000;,
    0.875000; 0.125000;,
    0.937500; 0.125000;,
    0.997070; 0.125000;,
    0.031250; 0.000000;,
    0.093750; 0.000000;,
    0.156250; 0.000000;,
    0.218750; 0.000000;,
    0.281250; 0.000000;,
    0.343750; 0.000000;,
    0.406250; 0.000000;,
    0.468750; 0.000000;,
    0.531250; 0.000000;,
    0.593750; 0.000000;,
    0.656250; 0.000000;,
    0.718750; 0.000000;,
    0.781250; 0.000000;,
    0.843750; 0.000000;,
    0.906250; 0.000000;,
    0.971680; -0.001953;;
   }

   MeshVertexColors {
    156;
    0; 1.000000; 1.000000; 1.000000; 1.000000;,
    1; 1.000000; 1.000000; 1.000000; 1.000000;,
    2; 1.000000; 1.000000; 1.000000; 1.000000;,
    3; 1.000000; 1.000000; 1.000000; 1.000000;,
    4; 1.000000; 1.000000; 1.000000; 1.000000;,
    5; 1.000000; 1.000000; 1.000000; 1.000000;,
    6; 1.000000; 1.000000; 1.000000; 1.000000;,
    7; 1.000000; 1.000000; 1.000000; 1.000000;,
    8; 1.000000; 1.000000; 1.000000; 1.000000;,
    9; 1.000000; 1.000000; 1.000000; 1.000000;,
    10; 1.000000; 1.000000; 1.000000; 1.000000;,
    11; 1.000000; 1.000000; 1.000000; 1.000000;,
    12; 1.000000; 1.000000; 1.000000; 1.000000;,
    13; 1.000000; 1.000000; 1.000000; 1.000000;,
    14; 1.000000; 1.000000; 1.000000; 1.000000;,
    15; 1.000000; 1.000000; 1.000000; 1.000000;,
    16; 1.000000; 1.000000; 1.000000; 1.000000;,
    17; 1.000000; 1.000000; 1.000000; 1.000000;,
    18; 1.000000; 1.000000; 1.000000; 1.000000;,
    19; 1.000000; 1.000000; 1.000000; 1.000000;,
    20; 1.000000; 1.000000; 1.000000; 1.000000;,
    21; 1.000000; 1.000000; 1.000000; 1.000000;,
    22; 1.000000; 1.000000; 1.000000; 1.000000;,
    23; 1.000000; 1.000000; 1.000000; 1.000000;,
    24; 1.000000; 1.000000; 1.000000; 1.000000;,
    25; 1.000000; 1.000000; 1.000000; 1.000000;,
    26; 1.000000; 1.000000; 1.000000; 1.000000;,
    27; 1.000000; 1.000000; 1.000000; 1.000000;,
    28; 1.000000; 1.000000; 1.000000; 1.000000;,
    29; 1.000000; 1.000000; 1.000000; 1.000000;,
    30; 1.000000; 1.000000; 1.000000; 1.000000;,
    31; 1.000000; 1.000000; 1.000000; 1.000000;,
    32; 1.000000; 1.000000; 1.000000; 1.000000;,
    33; 1.000000; 1.000000; 1.000000; 1.000000;,
    34; 1.000000; 1.000000; 1.000000; 1.000000;,
    35; 1.000000; 1.000000; 1.000000; 1.000000;,
    36; 1.000000; 1.000000; 1.000000; 1.000000;,
    37; 1.000000; 1.000000; 1.000000; 1.000000;,
    38; 1.000000; 1.000000; 1.000000; 1.000000;,
    39; 1.000000; 1.000000; 1.000000; 1.000000;,
    40; 1.000000; 1.000000; 1.000000; 1.000000;,
    41; 1.000000; 1.000000; 1.000000; 1.000000;,
    42; 1.000000; 1.000000; 1.000000; 1.000000;,
    43; 1.000000; 1.000000; 1.000000; 1.000000;,
    44; 1.000000; 1.000000; 1.000000; 1.000000;,
    45; 1.000000; 1.000000; 1.000000; 1.000000;,
    46; 1.000000; 1.000000; 1.000000; 1.000000;,
    47; 1.000000; 1.000000; 1.000000; 1.000000;,
    48; 1.000000; 1.000000; 1.000000; 1.000000;,
    49; 1.000000; 1.000000; 1.000000; 1.000000;,
    50; 1.000000; 1.000000; 1.000000; 1.000000;,
    51; 1.000000; 1.000000; 1.000000; 1.000000;,
    52; 1.000000; 1.000000; 1.000000; 1.000000;,
    53; 1.000000; 1.000000; 1.000000; 1.000000;,
    54; 1.000000; 1.000000; 1.000000; 1.000000;,
    55; 1.000000; 1.000000; 1.000000; 1.000000;,
    56; 1.000000; 1.000000; 1.000000; 1.000000;,
    57; 1.000000; 1.000000; 1.000000; 1.000000;,
    58; 1.000000; 1.000000; 1.000000; 1.000000;,
    59; 1.000000; 1.000000; 1.000000; 1.000000;,
    60; 1.000000; 1.000000; 1.000000; 1.000000;,
    61; 1.000000; 1.000000; 1.000000; 1.000000;,
    62; 1.000000; 1.000000; 1.000000; 1.000000;,
    63; 1.000000; 1.000000; 1.000000; 1.000000;,
    64; 1.000000; 1.000000; 1.000000; 1.000000;,
    65; 1.000000; 1.000000; 1.000000; 1.000000;,
    66; 1.000000; 1.000000; 1.000000; 1.000000;,
    67; 1.000000; 1.000000; 1.000000; 1.000000;,
    68; 1.000000; 1.000000; 1.000000; 1.000000;,
    69; 1.000000; 1.000000; 1.000000; 1.000000;,
    70; 1.000000; 1.000000; 1.000000; 1.000000;,
    71; 1.000000; 1.000000; 1.000000; 1.000000;,
    72; 1.000000; 1.000000; 1.000000; 1.000000;,
    73; 1.000000; 1.000000; 1.000000; 1.000000;,
    74; 1.000000; 1.000000; 1.000000; 1.000000;,
    75; 1.000000; 1.000000; 1.000000; 1.000000;,
    76; 1.000000; 1.000000; 1.000000; 1.000000;,
    77; 1.000000; 1.000000; 1.000000; 1.000000;,
    78; 1.000000; 1.000000; 1.000000; 1.000000;,
    79; 1.000000; 1.000000; 1.000000; 1.000000;,
    80; 1.000000; 1.000000; 1.000000; 1.000000;,
    81; 1.000000; 1.000000; 1.000000; 1.000000;,
    82; 1.000000; 1.000000; 1.000000; 1.000000;,
    83; 1.000000; 1.000000; 1.000000; 1.000000;,
    84; 1.000000; 1.000000; 1.000000; 1.000000;,
    85; 1.000000; 1.000000; 1.000000; 1.000000;,
    86; 1.000000; 1.000000; 1.000000; 1.000000;,
    87; 1.000000; 1.000000; 1.000000; 1.000000;,
    88; 1.000000; 1.000000; 1.000000; 1.000000;,
    89; 1.000000; 1.000000; 1.000000; 1.000000;,
    90; 1.000000; 1.000000; 1.000000; 1.000000;,
    91; 1.000000; 1.000000; 1.000000; 1.000000;,
    92; 1.000000; 1.000000; 1.000000; 1.000000;,
    93; 1.000000; 1.000000; 1.000000; 1.000000;,
    94; 1.000000; 1.000000; 1.000000; 1.000000;,
    95; 1.000000; 1.000000; 1.000000; 1.000000;,
    96; 1.000000; 1.000000; 1.000000; 1.000000;,
    97; 1.000000; 1.000000; 1.000000; 1.000000;,
    98; 1.000000; 1.000000; 1.000000; 1.000000;,
    99; 1.000000; 1.000000; 1.000000; 1.000000;,
    100; 1.000000; 1.000000; 1.000000; 1.000000;,
    101; 1.000000; 1.000000; 1.000000; 1.000000;,
    102; 1.000000; 1.000000; 1.000000; 1.000000;,
    103; 1.000000; 1.000000; 1.000000; 1.000000;,
    104; 1.000000; 1.000000; 1.000000; 1.000000;,
    105; 1.000000; 1.000000; 1.000000; 1.000000;,
    106; 1.000000; 1.000000; 1.000000; 1.000000;,
    107; 1.000000; 1.000000; 1.000000; 1.000000;,
    108; 1.000000; 1.000000; 1.000000; 1.000000;,
    109; 1.000000; 1.000000; 1.000000; 1.000000;,
    110; 1.000000; 1.000000; 1.000000; 1.000000;,
    111; 1.000000; 1.000000; 1.000000; 1.000000;,
    112; 1.000000; 1.000000; 1.000000; 1.000000;,
    113; 1.000000; 1.000000; 1.000000; 1.000000;,
    114; 1.000000; 1.000000; 1.000000; 1.000000;,
    115; 1.000000; 1.000000; 1.000000; 1.000000;,
    116; 1.000000; 1.000000; 1.000000; 1.000000;,
    117; 1.000000; 1.000000; 1.000000; 1.000000;,
    118; 1.000000; 1.000000; 1.000000; 1.000000;,
    119; 1.000000; 1.000000; 1.000000; 1.000000;,
    120; 1.000000; 1.000000; 1.000000; 1.000000;,
    121; 1.000000; 1.000000; 1.000000; 1.000000;,
    122; 1.000000; 1.000000; 1.000000; 1.000000;,
    123; 1.000000; 1.000000; 1.000000; 1.000000;,
    124; 1.000000; 1.000000; 1.000000; 1.000000;,
    125; 1.000000; 1.000000; 1.000000; 1.000000;,
    126; 1.000000; 1.000000; 1.000000; 1.000000;,
    127; 1.000000; 1.000000; 1.000000; 1.000000;,
    128; 1.000000; 1.000000; 1.000000; 1.000000;,
    129; 1.000000; 1.000000; 1.000000; 1.000000;,
    130; 1.000000; 1.000000; 1.000000; 1.000000;,
    131; 1.000000; 1.000000; 1.000000; 1.000000;,
    132; 1.000000; 1.000000; 1.000000; 1.000000;,
    133; 1.000000; 1.000000; 1.000000; 1.000000;,
    134; 1.000000; 1.000000; 1.000000; 1.000000;,
    135; 1.000000; 1.000000; 1.000000; 1.000000;,
    136; 1.000000; 1.000000; 1.000000; 1.000000;,
    137; 1.000000; 1.000000; 1.000000; 1.000000;,
    138; 1.000000; 1.000000; 1.000000; 1.000000;,
    139; 1.000000; 1.000000; 1.000000; 1.000000;,
    140; 1.000000; 1.000000; 1.000000; 1.000000;,
    141; 1.000000; 1.000000; 1.000000; 1.000000;,
    142; 1.000000; 1.000000; 1.000000; 1.000000;,
    143; 1.000000; 1.000000; 1.000000; 1.000000;,
    144; 1.000000; 1.000000; 1.000000; 1.000000;,
    145; 1.000000; 1.000000; 1.000000; 1.000000;,
    146; 1.000000; 1.000000; 1.000000; 1.000000;,
    147; 1.000000; 1.000000; 1.000000; 1.000000;,
    148; 1.000000; 1.000000; 1.000000; 1.000000;,
    149; 1.000000; 1.000000; 1.000000; 1.000000;,
    150; 1.000000; 1.000000; 1.000000; 1.000000;,
    151; 1.000000; 1.000000; 1.000000; 1.000000;,
    152; 1.000000; 1.000000; 1.000000; 1.000000;,
    153; 1.000000; 1.000000; 1.000000; 1.000000;,
    154; 1.000000; 1.000000; 1.000000; 1.000000;,
    155; 1.000000; 1.000000; 1.000000; 1.000000;;
   }

   MeshMaterialList {
    1;
    224;
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0,
    0;

    Material mat_planet6_1 {
     0.800000; 0.800000; 0.800000; 1.000000;;
     128.000000;
     0.000000; 0.000000; 0.000000;;
     0.000000; 0.000000; 0.000000;;

     TextureFilename {
      "mat_plan.bmp";
     }
    }

   }
  }
}
