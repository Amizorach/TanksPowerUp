# TanksPowerUp

ML-Agent Tanks learn to drive and pick up power ups or die


This is a simple project intended to be used as a fast start for learning and devloping on the Unity MLAgent platform.

The code is built in a way that allows changing many configurtaions with out effecting the basic agent too much.

Setup:
* Full Project 
  * Download Git 
  * Open the project 
  * Open SceneAgents 
  * Press play 
  
  
* As Asset 
  * Download git
  * Open new unity package 
  * install ML agents package from PackageManager 
  * import assets under tanks into project 
  * Open SceneAgents 
  * Press Play
  
The Project comes with a trained neural network

If you want to train a new network 
Make sure you have conda installed 
and that you copy the config directory to your project folder (If you imported the full project it should already be under the root folder)
run from conda terminal at project root 

**mlagents-learn config/YTTanks.yaml --run-id=xxx**

you can then see the results in TensorBoard  using 

**tensorboard --logdir=results/**

See documents for Tensorboard scalars included or check the driver scalars in the Tensorboard Webpage


# Credits
This Project uses models from Unity's tanks tutorial 
The final prefabs have been modefied but the base credit for the models them self belongs to the original creator 


