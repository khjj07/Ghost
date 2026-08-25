# Ghost

열쇠·문·거울 반사 기믹을 활용한 2D 탑다운 퍼즐 프로토타입입니다.

**개발 기간**: 2022.10 ~ 2022.10

## 사용 엔진

- Unity (C#, URP, 2D Tilemap)

## 핵심 기술

- 타일맵 기반 2D 스테이지 구성
- 열쇠-문(Key/Door) 개폐 퍼즐 로직
- 거울 반사(MirrorRotation) 기반 퍼즐 기믹
- 유령(Ghost) 캐릭터를 활용한 스테이지 연출

## 기여 개요

2인 팀 프로젝트로 참여하여 플레이어/유령 캐릭터 로직, 열쇠-문 기믹, 거울 반사 퍼즐, 스테이지 맵 배치를 담당했습니다. 초기 프로토타입 단계의 프로젝트입니다.

## 프로젝트 구조

```
Assets/
├─ Scenes, Prefab, Sprites, TileMaps, Animation
└─ Script/
   ├─ GameManager.cs
   ├─ Player.cs
   ├─ Ghost.cs
   ├─ Door.cs / Key.cs
   ├─ MirrorRotation.cs
   ├─ Title.cs
   └─ Prop/
```
