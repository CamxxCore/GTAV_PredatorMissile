using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTA;
using GTA.Native;

public class DamageProofs
{
    private Entity _entity;
    private bool _bulletProof, _fireProof, _explosionProof, _collisionProof, _meleeProof, _unkProof, _unkProof1, _drownProof;

    #region Public Variables

    public bool BulletProof
    {
        get { return _bulletProof; }
        set { _bulletProof = value; }
    }

    public bool FireProof
    {
        get { return _fireProof; }
        set
        {
            _fireProof = value;
        }
    }

    public bool ExplosionProof
    {
        get { return _explosionProof; }
        set
        {
            _explosionProof = value;
        }
    }

    public bool CollisionProof
    {
        get { return _collisionProof; }
        set
        {
            _collisionProof = value;
        }
    }

    public bool MeleeProof
    {
        get { return _meleeProof; }
        set
        {
            _meleeProof = value;
        }
    }

    public bool UnkProof
    {
        get { return _unkProof; }
        set
        {
            _unkProof = value;
        }
    }

    public bool UnkProof1
    {
        get { return _unkProof1; }
        set
        {
            _unkProof1 = value;
        }
    }

    public bool DrownProof
    {
        get { return _drownProof; }
        set
        {
            _bulletProof = value;
        }
    }

    #endregion

    public DamageProofs(Entity entity) : this(entity, false, false, false, false, false, false, false, false)
    { }

    public DamageProofs(Entity entity, bool bulletProof, bool fireProof, bool explosionProof, bool collisionProof, bool meleeProof, bool unk, bool unk1, bool drownProof)
    {
        _entity = entity;
        _bulletProof = bulletProof;
        _fireProof = fireProof;
        _explosionProof = explosionProof;
        _collisionProof = collisionProof;
        _meleeProof = meleeProof;
        _unkProof = unk;
        _unkProof1 = unk1;
        _drownProof = drownProof;
    }

    public void SetAll()
    {
        Function.Call(Hash.SET_ENTITY_PROOFS, _entity.Handle, _bulletProof, _fireProof, _explosionProof, _collisionProof, _meleeProof, _unkProof, _unkProof1, _drownProof);
    }
}