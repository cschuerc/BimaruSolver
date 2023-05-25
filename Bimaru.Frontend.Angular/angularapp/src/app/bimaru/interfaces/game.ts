import { GridTile } from "./gridTile";

export interface Game {
    NumberOfRows: number;
    NumberOfColumns: number;
    TargetNumberOfShipFieldsPerRow: number[];
    TargetNumberOfShipFieldsPerColumn: number[];
    TargetNumberOfShipsPerLength: number[];
    GridTiles: GridTile[][];
}