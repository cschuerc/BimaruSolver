import { BimaruValue } from "./bimaruValue";

export interface Game {
    NumberOfRows: number;
    NumberOfColumns: number;
    TargetNumberOfShipFieldsPerRow: number[];
    TargetNumberOfShipFieldsPerColumn: number[];
    TargetNumberOfShipsPerLength: number[];
    GridValues: BimaruValue[][];
}