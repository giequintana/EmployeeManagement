import { useEffect } from 'react';

const APP_NAME = 'Employee Management';

function PageHeader({ title, subtitle, actions }) {
  useEffect(() => {
    if (typeof title === 'string') {
      document.title = `${title} | ${APP_NAME}`;
    }
  }, [title]);

  return (
    <div className="d-flex flex-wrap justify-content-between align-items-center gap-2 mb-4">
      <div>
        <h1 className="h3 mb-0">{title}</h1>
        {subtitle && <div className="text-body-secondary">{subtitle}</div>}
      </div>
      {actions && <div className="d-flex flex-wrap gap-2">{actions}</div>}
    </div>
  );
}

export default PageHeader;