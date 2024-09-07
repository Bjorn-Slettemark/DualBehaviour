//using System.Collections.Generic;

//public class SyncVarWrapper<T>
//{
//    private T value;
//    private MultiBehaviour owner;
//    private string fieldName;

//    public SyncVarWrapper(MultiBehaviour owner, string fieldName, T initialValue)
//    {
//        this.owner = owner;
//        this.fieldName = fieldName;
//        this.value = initialValue;
//    }

//    public T Value
//    {
//        get => value;
//        set
//        {
//            if (!EqualityComparer<T>.Default.Equals(this.value, value))
//            {
//                this.value = value;
//                owner.OnSyncVarChanged(fieldName, this.value);
//            }
//        }
//    }

//    public static implicit operator T(SyncVarWrapper<T> wrapper) => wrapper.Value;

//    public object GetWrappedValue() => Value;
//}