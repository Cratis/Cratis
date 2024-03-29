// Copyright (c) Cratis. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

import React from 'react';

export interface ITypography extends React.HTMLAttributes<HTMLHeadingElement> {
    headingVariant: 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'p';
}

export const Typography = ({
    headingVariant = 'p',
    children,
    ...restProps
}: ITypography) => {
    const HeadingTag = headingVariant as 'h1' | 'h2' | 'h3' | 'h4' | 'h5' | 'p';

    return <HeadingTag {...restProps}>{children}</HeadingTag>;
};
