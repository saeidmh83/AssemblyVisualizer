# Assembly Visualizer 

*Assembly Visualizer* is a data visualization plugin for .NET decompilers, currently supporting ILSpy and Reflector.    

Website: http://visualizer.denismarkelov.com    
Copyright 2011 Denis Markelov    
Licensed under the Microsoft Public License

## Included open-source libraries

* GraphSharp: Ms-PL  
* QuickGraph: Ms-PL  
* WPFExtensions: Ms-PL

## Components  

### Assembly browser
    
Type hierarchies visualizer, starting from the superclass.    
Usage: 'Browse Assembly' context menu item for assemblies, 
'Visualize Descendants' context menu item for types.

### Ancestry browser
    
Type hierarchy visualizer from selected type to the inheritance root.    
Usage: 'Browse Ancestry' context menu item for types.

### Dependency browser

Assembly references visualizer.    
Usage: 'Browse Dependencies' context menu item for assemblies.

## How to start to use a plugin  

Download the latest build from the Downloads section, put assembly into the folder with your decompiler's    
executable. For Reflector you also need to add it explicitly at Tools -> Add-Ins window.
  