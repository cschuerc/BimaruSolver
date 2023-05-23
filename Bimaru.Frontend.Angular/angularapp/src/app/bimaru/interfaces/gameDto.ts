import { BimaruValue } from "./bimaruValue"
import { GridValueDto } from "./gridValueDto";

export interface GameDto {
    NumberOfRows: number;
    NumberOfColumns: number;
    TargetNumberOfShipFieldsPerRow: number[];
    TargetNumberOfShipFieldsPerColumn: number[];
    TargetNumberOfShipsPerLength: number[];
    GridValues: GridValueDto[];
}