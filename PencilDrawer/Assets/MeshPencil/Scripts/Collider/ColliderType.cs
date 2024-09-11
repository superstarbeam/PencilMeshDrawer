namespace MeshPencil.Collider
{
    public enum ColliderType
    {
        None = 0,
        StaticCollider = 1, //Mesh collider for kinematic rigidbody
        DynamicCollider = 2 //Generate dynamic mesh collider for non kinematic rigidbody support
    }
}
