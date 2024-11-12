import { Dropdown } from "@mui/base/Dropdown";
import { MenuButton } from "@mui/base/MenuButton";
import { Menu, MenuListboxSlotProps } from "@mui/base/Menu";
import { MenuItem, menuItemClasses } from "@mui/base/MenuItem";

import { forwardRef } from "react";
import { CssTransition } from "@mui/base/Transitions";

import { styled } from "@mui/system";

export default function DateUnitButton() {
  const createHandleMenuClick = (menuItem: string) => {
    return () => {
      console.log(`Clicked on ${menuItem}`);
    };
  };
  return (
    <Dropdown>
      <MenuButtonStyled>選擇時間單位</MenuButtonStyled>
      <Menu slots={{ listbox: AnimatedListbox }}>
        <MenuItemStyled onClick={createHandleMenuClick("Profile")}>
          Profile
        </MenuItemStyled>
        <MenuItemStyled onClick={createHandleMenuClick("Language settings")}>
          Language settings
        </MenuItemStyled>
        <MenuItemStyled onClick={createHandleMenuClick("Log out")}>
          Log out
        </MenuItemStyled>
      </Menu>
    </Dropdown>
  );
}

const blue = {
  50: "#F0F7FF",
  100: "#C2E0FF",
  200: "#99CCF3",
  300: "#66B2FF",
  400: "#3399FF",
  500: "#007FFF",
  600: "#0072E6",
  700: "#0059B3",
  800: "#004C99",
  900: "#003A75",
};

const grey = {
  50: "#F3F6F9",
  100: "#E5EAF2",
  200: "#DAE2ED",
  300: "#C7D0DD",
  400: "#B0B8C4",
  500: "#9DA8B7",
  600: "#6B7A90",
  700: "#434D5B",
  800: "#303740",
  900: "#1C2025",
};
const Listbox = styled("ul")(
  () => `
    font-family: 'IBM Plex Sans', sans-serif;
    font-size: 0.875rem;
    box-sizing: border-box;
    padding: 6px;
    margin: 12px 0;
    min-width: 200px;
    border-radius: 12px;
    overflow: auto;
    outline: 0;
    background: #fff;
    border: 1px solid ${grey[200]};
    color: ${grey[900]};
    box-shadow: 0px 4px 30px ${grey[200]};
    z-index: 1;
  
    .closed & {
      opacity: 0;
      transform: scale(0.95, 0.8);
      transition: opacity 200ms ease-in, transform 200ms ease-in;
    }
    
    .open & {
      opacity: 1;
      transform: scale(1, 1);
      transition: opacity 100ms ease-out, transform 100ms cubic-bezier(0.43, 0.29, 0.37, 1.48);
    }
  
    .placement-top & {
      transform-origin: bottom;
    }
  
    .placement-bottom & {
      transform-origin: top;
    }
    `,
);

const AnimatedListbox = forwardRef(function AnimatedListbox(
  props: MenuListboxSlotProps,
  ref: React.ForwardedRef<HTMLUListElement>,
) {
  const { ...other } = props;

  return (
    <CssTransition enterClassName="open" exitClassName="closed">
      <Listbox {...other} ref={ref} />
    </CssTransition>
  );
});

const MenuItemStyled = styled(MenuItem)(
  () => `
    list-style: none;
    padding: 8px;
    border-radius: 8px;
    cursor: default;
    user-select: none;
  
    &:last-of-type {
      border-bottom: none;
    }
  
     &:hover {
      background: ${grey[50]};
      border-color: ${grey[300]};
    }

    &:focus {
      outline: 3px solid ${blue[200]};
      background-color: ${grey[100]};
      color: ${grey[900]};
    }
  
    &.${menuItemClasses.disabled} {
      color: ${grey[400]};
    }
    `,
);

const MenuButtonStyled = styled(MenuButton)(
  () => `
  
    font-family: 'IBM Plex Sans', sans-serif;
    font-weight: 600;
    font-size: 0.875rem;
    line-height: 1.5;
    padding: 8px 16px;
    border-radius: 8px;
    color: white;
    transition: all 150ms ease;
    cursor: pointer;
    background: #fff;
    border: 1px solid ${grey[200]};
    color: ${grey[900]};
    box-shadow: 0 1px 2px 0 rgb(0 0 0 / 0.05);
  
    &:hover {
      background: ${grey[50]};
      border-color: ${grey[300]};
    }
  
    &:active {
      background: ${grey[100]};
    }
  
    &:focus-visible {
      box-shadow: 0 0 0 4px ${blue[200]};
      outline: none;
    }

    `,
);
