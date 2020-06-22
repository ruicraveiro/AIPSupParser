# AIPSupParser

## Introduction

The authority in charge of the Portuguese airspace, Nav Portugal, issued an AIP supplement, 031/2020, containing the airspace limits 
where fire surveillance drones will be operating.

The format of the document includes sections of areas that define polygons. The problem with this supplement is that is wasn't issued
as a notam, at least not all at once, so these areas are not being displayed by EFB applications.

AIPSupParser is just a simple application that parses the text of the AIP and generates a roughly corresponding KML file content.

## Usage

It is a very basic WPF application put together in a rush (no Prism, MVVM, DI, whatsoever). Simply run the application, copy the contents 
from the AIP beginning at the second area (AREA N1), all the way down to the last area (AREA S10), paste them on the top textbox,  press "Generate" and the KML contents 
appear in the bottom text-box. Save those contents to a text file with a KML extension.

## Rough Edges

1. The application is incapable to parsing the very first section, because it defines a circle. Some more work would be needed to support this feature.
2. Some areas have a border running along the border between two coordinates. The generated KML will only draw a straight line between 
the two coordinates as it does not have the capability to run along the border.
