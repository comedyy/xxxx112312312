using System;
using System.Collections.Generic;
using Unity.Entities;

public class EcsSystemNodeDescriptor
{
    public Type rootType;
    public List<EcsSystemNodeDescriptor> list;
}

public class EcsSystemList
{
    public static List<EcsSystemNodeDescriptor> nodeDescriptorList = new List<EcsSystemNodeDescriptor>()
    {
        new EcsSystemNodeDescriptor
        {
            rootType = typeof(InitializationSystemGroup),
            list = new List<EcsSystemNodeDescriptor>
            {
                new EcsSystemNodeDescriptor{ rootType = typeof(ControllerUserSystem)},
                new EcsSystemNodeDescriptor{ rootType = typeof(UpdateLocalFrameSystem)}
            }
        },
        new EcsSystemNodeDescriptor
        {
            rootType = typeof(SimulationSystemGroup),
            list = new List<EcsSystemNodeDescriptor>
            {
                new EcsSystemNodeDescriptor{
                    rootType = typeof(LogicUpdateSystemGroup),
                    list = new List<EcsSystemNodeDescriptor>
                    {
                        new EcsSystemNodeDescriptor{ rootType = typeof(InputUserSystem)},
                        new EcsSystemNodeDescriptor{ rootType = typeof(PreRvoSystemGroup)},
                        new EcsSystemNodeDescriptor{ rootType = typeof(RvoSystemGroup)},
                        new EcsSystemNodeDescriptor{ rootType = typeof(AfterRvoSystemGroup)},

                        new EcsSystemNodeDescriptor{ rootType = typeof(CalculateHashSystem)},
                        new EcsSystemNodeDescriptor{ rootType = typeof(CompareHashSystem)},
                        new EcsSystemNodeDescriptor{ rootType = typeof(WritePlaybackDataSystem)},
                    }
                }
            }
        },
        new EcsSystemNodeDescriptor
        {
            rootType = typeof(PresentationSystemGroup),

            list = new List<EcsSystemNodeDescriptor>{ 
                new EcsSystemNodeDescriptor{
                    rootType = typeof(UnsortedPresentationSystemGroup),

                    list = new List<EcsSystemNodeDescriptor>
                    {
                        new EcsSystemNodeDescriptor{ rootType = typeof(VLerpTransformSystem)},
                        new EcsSystemNodeDescriptor{ rootType = typeof(VSyncGameObjectPositionSystem)},
                        new EcsSystemNodeDescriptor{ rootType = typeof(DrawEntitySystem)}
                    }
                }
            }
        },    
    };
}