///<summary>
///接口实现观察者模式的订阅和广播
///</summary>
public interface IEndGameObserver//订阅者接口类，每一个订阅者都是观察者
{
    void EndNotify();//订阅者接受函数
}
